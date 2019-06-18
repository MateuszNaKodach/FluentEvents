using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.Hosting;
using WorkerSample.DomainModel;
using WorkerSample.Events;
using WorkerSample.Notifications;

namespace WorkerSample.Worker
{
    public class NotificationsHostedService : IHostedService
    {
        private readonly AppEventsContext _appEventsContext;
        private readonly IMailService _mailService;

        private UnsubscribeToken _unsubscribeToken;

        public NotificationsHostedService(AppEventsContext appEventsContext, IMailService mailService)
        {
            _appEventsContext = appEventsContext;
            _mailService = mailService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _unsubscribeToken = _appEventsContext
                .SubscribeGloballyTo<ProductSubscription>(productSubscription =>
                {
                    productSubscription.Cancelled += ProductSubscriptionOnCancelled;
                });

            return Task.CompletedTask;
        }

        private void ProductSubscriptionOnCancelled(object sender, ProductSubscriptionCancelledEventArgs e)
        {
            var productSubscription = (ProductSubscription) sender;
            _mailService.SendSubscriptionCancelledEmail(productSubscription.CustomerEmailAddress);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _appEventsContext.Unsubscribe(_unsubscribeToken);

            return Task.CompletedTask;
        }
    }
}
