using System.Threading;
using System.Threading.Tasks;
using AspNetCoreApiSample.Events;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreApiSample.Notifications
{
    public class NotificationsHostedService : IHostedService
    {
        private readonly AppEventsContext _appEventsContext;
        private readonly IMailService _mailService;
        private ISubscriptionsCancellationToken _subscriptionsCancellationToken;

        public NotificationsHostedService(AppEventsContext appEventsContext, IMailService mailService)
        {
            _appEventsContext = appEventsContext;
            _mailService = mailService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriptionsCancellationToken = _appEventsContext.SubscribeGloballyTo<ContractEvents>(contract =>
            {
                contract.Terminated += ContractOnTerminated;
            });

            return Task.CompletedTask;
        }

        private async Task ContractOnTerminated(object sender, ContractEvents.ContractTerminatedEventArgs eventArgs)
        {
            var contract = (ContractEvents) sender;

            await _mailService.SendContractTerminatedEmailAsync(contract.Id, eventArgs.Reason);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _appEventsContext.Unsubscribe(_subscriptionsCancellationToken);

            return Task.CompletedTask;
        }
    }
}