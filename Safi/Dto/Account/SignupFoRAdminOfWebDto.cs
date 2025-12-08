namespace Safi.Dto.Account
{
    public class SignupFoRAdminOfWebDto
    {
        public string username { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public IFormFile? Image { get; set; }
    }
}

