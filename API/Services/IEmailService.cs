using API.DTOs;

namespace API.Services
{
    public interface IEmailService
    {
        bool SendEmail(EmailData emailData);
    }
}