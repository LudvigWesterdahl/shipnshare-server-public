using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.ExternalServices;

namespace ShipWithMeInfrastructure.Services
{
    /// <summary>
    /// Implementation for <see cref="IEmailService"/>.
    /// </summary>
    public sealed class EmailService : IEmailService
    {
        private readonly ILogger<IEmailService> logger;

        private readonly NETCore.MailKit.Core.IEmailService emailService;

        public EmailService(ILogger<IEmailService> logger, NETCore.MailKit.Core.IEmailService emailService)
        {
            this.logger = logger;
            this.emailService = emailService;
        }

        /// <inheritdoc cref="IEmailService.Send(string, string, string)"/>
        public async Task<bool> Send(string title, string message, string toEmail)
        {
            try
            {
                await emailService.SendAsync(toEmail, title, message, isHtml: true);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to send email.\nTitle={}\nMessage={}\nToEmail={}\nException={}", title, message, toEmail, e.Message);
                return false;
            }

            return true;
        }
    }
}
