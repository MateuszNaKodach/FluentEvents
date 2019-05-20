using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCoreApiSample.Notifications
{
    public class MailService : IMailService 
    {
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public Task SendContractTerminatedEmailAsync(int contractId, string reason)
        {
            _logger.LogWarning($"Contract terminated email sent (Contract Id: {contractId}, reason: {reason})");    

            return Task.CompletedTask;
        }
    }
}
