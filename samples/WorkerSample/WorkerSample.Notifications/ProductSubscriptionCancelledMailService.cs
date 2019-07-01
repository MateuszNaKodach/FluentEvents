using System.Threading.Tasks;
using FluentEvents;
using WorkerSample.Domain;

namespace WorkerSample.Notifications
{
    public class ProductSubscriptionCancelledMailService 
        : IEventHandler<ProductSubscription, ProductSubscriptionCancelledEventArgs>
    {
        private readonly IMailService _mailService;

        public ProductSubscriptionCancelledMailService(IMailService mailService)
        {
            _mailService = mailService;
        }

        public async Task HandleEventAsync(ProductSubscription source, ProductSubscriptionCancelledEventArgs args)
        {
            await _mailService.SendEmailAsync(
                source.CustomerEmailAddress,
                "Subscription cancelled",
                "Your subscription was cancelled"
            );
        }
    }
}
