namespace Safi.Dto.Account
{
    public class ResetPasswordDto
    {
        public string Password { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
