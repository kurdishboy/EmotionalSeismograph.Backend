namespace EmotionalSeismograph.Backend.Models
{
    public class User
    {
        public long id { get; set; }
        public required string googleId { get; set; }
        public required string email { get; set; }
        public required string name { get; set; }
        public string? profilePictureUrl { get; set; }
        public string? refreshToken { get; set; }
        public DateTime? tokenCreated { get; set; }
        public DateTime? tokenExpires { get; set; }
    }

    public class UserAuthenticationResponse
    {
        public string? token { get; set; }
        public string? refreshToken { get; set; }
    }
    public class RefreshToken
    {
        public required string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; }
    }
}
