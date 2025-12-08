using System.Text.Json.Serialization;

namespace Safi.Dto.Account
{
    public class ResponseOfLogin
    {
        public string? Username { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Email { get; set; }
        public string? Id { get; set; }
        public int ?Custom_Id { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
