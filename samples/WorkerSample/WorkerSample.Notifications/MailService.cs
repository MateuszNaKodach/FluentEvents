using Microsoft.Extensions.Logging;

namespace WorkerSample.Notifications
{
    public class MailService : IMailService
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