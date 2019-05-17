using Microsoft.Extensions.Logging;

namespace WorkerSample.Mail
{
    internal class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public void SendSubscriptionCancelledEmail(string emailAddress)
        {
            _logger.LogWarning($"Subscription cancelled email sent to {emailAddress}.");
        }
    }
}