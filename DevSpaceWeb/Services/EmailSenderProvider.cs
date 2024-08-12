using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Identity;

namespace DevSpaceWeb.Services
{
    public class EmailSender : IEmailSender<AuthUser>
    {
        public EmailSender(EmailService emailService)
        {
            _emailService = emailService;
        }

        private readonly EmailService _emailService;

        public Task SendConfirmationLinkAsync(AuthUser user, string email,
            string confirmationLink) => _emailService.Send(SendMailType.AccountConfirm, user, confirmationLink);

        public Task SendPasswordResetLinkAsync(AuthUser user, string email,
            string resetLink) => _emailService.Send(SendMailType.AccountPasswordChangeConfirm, user, resetLink);

        public Task SendPasswordResetCodeAsync(AuthUser user, string email,
            string resetCode) => _emailService.Send(SendMailType.AccountPasswordChangeCode, user, resetCode);


    }
}
