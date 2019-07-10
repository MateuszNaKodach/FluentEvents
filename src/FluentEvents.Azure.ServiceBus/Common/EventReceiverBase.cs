using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Azure.ServiceBus.Common
{
    internal abstract class EventReceiverBase : IEventReceiver
    {
        private readonly ILogger _logger;
        private readonly IEventsSerializationService _eventsSerializationService;
        private readonly IPublishingService _publishingService;
        private readonly EventReceiverConfigBase _config;

        private IReceiverClient _subscriptionClient;

        protected EventReceiverBase(
            ILogger logger, 
            IEventsSerializationService eventsSerializationService,
            IPublishingService publishingService,
            EventReceiverConfigBase config
        )
        {
            _logger = logger;
            _eventsSerializationService = eventsSerializationService;
            _publishingService = publishingService;
            _config = config;
        }

        protected internal abstract Task<IReceiverClient> CreateReceiverClientAsync(CancellationToken cancellationToken);

        public async Task StartReceivingAsync(CancellationToken cancellationToken = default)
        {
            _subscriptionClient = await CreateReceiverClientAsync(cancellationToken).ConfigureAwait(false);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = _config.MaxConcurrentMessages
            };

            _subscriptionClient.RegisterMessageHandler(HandleMessageAsync, messageHandlerOptions);
        }

        public Task StopReceivingAsync(CancellationToken cancellationToken = default)
        {
            return _subscriptionClient.CloseAsync();
        }

        private async Task HandleMessageAsync(Message message, CancellationToken token)
        {
            try
            {
                var entityEvent = _eventsSerializationService.DeserializeEvent(message.Body);

                await _publishingService.PublishEventToGlobalSubscriptionsAsync(entityEvent).ConfigureAwait(false);

                await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _subscriptionClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);

                _logger.MessagesProcessingThrew(ex, message.MessageId);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.ServiceBusExceptionReceived(
                exceptionReceivedEventArgs.Exception,
                exceptionReceivedEventArgs.ExceptionReceivedContext
            );

            return Task.CompletedTask;
        }
    }
}
