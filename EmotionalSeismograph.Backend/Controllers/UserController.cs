using Core.Interfaces;
using Core.Services;
using EmotionalSeismograph.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace EmotionalSeismograph.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        IDAL _idal;
        public UserController(IDAL idal)
        {
            _idal = idal;
        }

        [HttpGet("getAllSpaces")]
        public async Task<IActionResult> getAllSpaces()
        {
            try
            {
                long? userId = long.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                string sql = "EXECUTE [dbo].[getAllSpaces] @userId = " + userId + "";
                string res = await _idal.executeReaderStream(sql);
                return Ok(CreateResponse.getResult(res, StatusCodes.Status200OK, null));
            }
            catch
            {
                return Ok(new CreateResponse<string>(null, StatusCodes.Status400BadRequest, null));
            }
        }

        [HttpPost("createSpace")]
        public async Task<IActionResult> createUser(Space space)
        {
            if (ModelState.IsValid)
            {
                long? userId = long.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                string sql = "EXECUTE [dbo].[createSpace] @name=N'" + space.name + "',@ownerUserId=" + userId + "";
                if (space.description != null)
                {
                    sql += ",@description=N'" + space.description + "'";
                }
                var result = await _idal.executenonquery(sql);
                if (result > 0)
                {
                    return Ok(new CreateResponse(null, StatusCodes.Status200OK, new List<string>() { }));
                }
                else
                {
                    return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, new List<string>() { "انجام نشد" }));
                }
            }
            else
            {

                return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
        }

        [HttpPost("joinSpace")]
        public async Task<IActionResult> joinSpace(SpaceUser spaceUser)
        {
            if (ModelState.IsValid)
            {
                long? userId = long.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                string sql = "EXECUTE [dbo].[joinSpace] @spaceId =" + spaceUser.spaceId + ", @userId =" + userId + "";
                var result = await _idal.executenonquery(sql);
                if (result > 0)
                {
                    return Ok(new CreateResponse(null, StatusCodes.Status200OK, new List<string>() { }));
                }
                else
                {
                    return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, new List<string>() { "انجام نشد" }));
                }
            }
            else
            {

                return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
        }

        [HttpPost("createEmotion")]
        public async Task<IActionResult> createEmotion(Emotion emotion)
        {
            if (ModelState.IsValid)
            {
                long? userId = long.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                string sql = "EXECUTE [dbo].[createEmotion] @userId = " + userId + ", @emotion = N'" + emotion.emotion + "' , @status =N'" + emotion.status + "' ";
                var result = await _idal.executenonquery(sql);
                if (result > 0)
                {
                    return Ok(new CreateResponse(null, StatusCodes.Status200OK, new List<string>() { }));
                }
                else
                {
                    return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, new List<string>() { "انجام نشد" }));
                }
            }
            else
            {

                return BadRequest(new CreateResponse(null, StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }
        }

        [HttpGet("getUserEmotions")]
        public async Task<IActionResult> getUserEmotions([FromQuery] string userId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                string sql = "EXECUTE [dbo].[getUserEmotions] @userId = " + userId + ", @startDate='" + startDate + "',@endDate='" + endDate + "'";
                string res = await _idal.executeReaderStream(sql);
                return Ok(CreateResponse.getResult(res, StatusCodes.Status200OK, null));
            }
            catch
            {
                return Ok(new CreateResponse<string>(null, StatusCodes.Status400BadRequest, null));
            }
        }

        [HttpGet("getSpaceUsersTopTenEmotions")]
        public async Task<IActionResult> getSpaceUsersTopTenEmotions([FromQuery] string spaceId)
        {
            try
            {
                string sql = "EXECUTE [dbo].[getSpaceUsersTopTenEmotions] @spaceId = " + spaceId + "";
                string res = await _idal.executeReaderStream(sql);
                return Ok(CreateResponse.getResult(res, StatusCodes.Status200OK, null));
            }
            catch
            {
                return Ok(new CreateResponse<string>(null, StatusCodes.Status400BadRequest, null));
            }
        }

    }
}
