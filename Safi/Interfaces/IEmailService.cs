using Safi.Dto.EmailDto;
namespace Safi.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(SendEmailDto Request);
    }
}
