using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkerSample.Notifications;

namespace WorkerSample.Worker
{
    internal class FakeMailService : IMailService
    {
        private readonly ILogger<FakeMailService> _logger;

        public FakeMailService(ILogger<FakeMailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string receiver, string subject, string body)
        {
            _logger.LogWarning("Email sent to {receiver}. Subject: {subject}. Body: {body}", receiver, subject, body);

            return Task.CompletedTask;
        }
    }
}
