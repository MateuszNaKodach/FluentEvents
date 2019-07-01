using System.Threading.Tasks;
using FluentEvents;
using WorkerSample.Domain;

namespace WorkerSample.Application
{
    public class ProductSubscriptionCancelledEventHandler 
        : IEventHandler<ProductSubscription, ProductSubscriptionCancelledEventArgs>
    {
        private readonly IMailService _mailService;

        public ProductSubscriptionCancelledEventHandler(IMailService mailService)
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
