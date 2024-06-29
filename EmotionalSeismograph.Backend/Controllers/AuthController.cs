using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Interfaces;
using EmotionalSeismograph.Backend.Models;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EmotionalSeismograph.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _configuration;
        IDAL _idal;
        public AuthController(IConfiguration config, IDAL idal)
        {
            _configuration = config;
            _idal = idal;
        }

        [HttpGet("login")]
        public IActionResult login([FromQuery]string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(HandleExternalLogin), "Auth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("handleexternallogin")]
        public async Task<IActionResult> HandleExternalLogin(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return BadRequest(); // Handle the error appropriately

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profilePictureUrl = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value; // Extract profile picture URL


            var userJsonString = await _idal.executeReaderStream("EXECUTE [dbo].[getUserByGoogleId] @googleId = N'" + googleId + "'");

            var createUserResult = 1;
            if (userJsonString == null || userJsonString.Trim() == "")
            {
                createUserResult = await _idal.executenonquery("EXECUTE [dbo].[createUser] @googleId = N'" + googleId + "',@email = N'" + email + "',@name = N'" + name + "',@profilePictureUrl = N'" + profilePictureUrl + "'");
                if (createUserResult > 0)
                {
                    userJsonString = await _idal.executeReaderStream("EXECUTE [dbo].[getUserByGoogleId] @googleId = N'" + googleId + "'");
                }
            }

            if (createUserResult >= 1)
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJsonString);
                string token = CreateToken(user.id, user.googleId, user.email, user.name, user.profilePictureUrl);
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, user.googleId);
                UserAuthenticationResponse userAuthenticationResponse = new UserAuthenticationResponse();
                userAuthenticationResponse.token = token;
                userAuthenticationResponse.refreshToken = refreshToken.Token;
                string jsonString = System.Text.Json.JsonSerializer.Serialize(userAuthenticationResponse);
                returnUrl += "?tokens=" + jsonString;
                return Redirect(returnUrl); // Redirect back to the frontend

            }
            else
            {
                return Ok(new CreateResponse<string>(null, StatusCodes.Status400BadRequest, null));
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<string>> RefreshToken(JsonElement data)
        {
            var refreshToken = data.GetProperty("refreshToken").ToString();
            if (refreshToken == null || refreshToken == string.Empty)
            {
                return Unauthorized();
            }


            string sql = "EXECUTE [dbo].[getUserByRefreshToken] @refreshToken = N'" + refreshToken + "'";
            string jsonResult = await _idal.executeReaderStream(sql);
            var user = JsonSerializer.Deserialize<User>(jsonResult);

            if (user == null)
            {
                return Unauthorized();
            }
            else if (user.name != null && user.name != string.Empty)
            {
                string token = CreateToken(user.id, user.googleId,user.email,user.name,user.profilePictureUrl);
                var newerfreshtoken = GenerateRefreshToken();
                SetRefreshToken(newerfreshtoken, user.googleId);

                UserAuthenticationResponse userAuthenticationResponse = new UserAuthenticationResponse();
                userAuthenticationResponse.token = token;
                userAuthenticationResponse.refreshToken = newerfreshtoken.Token;
                return Ok(new CreateResponse<UserAuthenticationResponse>(userAuthenticationResponse, StatusCodes.Status200OK, new List<string>() { }));
            }

            return Unauthorized();
        }

        [HttpGet("signout")]
        public async Task<ActionResult<string>> signout()
        {
            try
            {
                long? userId = long.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);

                if (userId == null)
                {
                    return Unauthorized();
                }

                string sql = "EXECUTE [dbo].[logoutUser] @userId = " + userId + "";
                await _idal.executeReaderStream(sql);
                return Ok(new CreateResponse<string>(" ", 200, null));
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpGet("authenticated")]
        [Authorize]
        public async Task<IActionResult> authenticated()
        {
            string? googleId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string sql = "EXECUTE [dbo].[getUserByGoogleId] @googleId = N'"+ googleId + "'";
            var userJsonString = await _idal.executeReaderStream(sql);
            if (userJsonString != null && userJsonString.Trim() != "")
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJsonString);

                if (user != null)
                {
                    return Ok(CreateResponse.getResult(userJsonString, StatusCodes.Status200OK, null));
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128)),
                Expired = DateTime.Now.AddDays(10)
            };
            return refreshToken;

        }

        private async void SetRefreshToken(RefreshToken newRefreshToken, string googleId)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expired,
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            string sql = "EXECUTE [dbo].[setRefreshToken] N'" + googleId + "', N'" + newRefreshToken.Token + "', '" + newRefreshToken.Expired + "', '" + newRefreshToken.Created + "'";
            await _idal.executenonquery(sql);
        }

        private string CreateToken(long userId, string googleId, string email, string name, string profilePictureUrl)
        {

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid , userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, googleId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim("urn:google:picture", profilePictureUrl)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cred

                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
