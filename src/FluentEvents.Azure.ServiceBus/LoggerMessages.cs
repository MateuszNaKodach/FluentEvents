using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Azure.ServiceBus
{
    internal static class LoggerMessages
    {
        private static readonly Action<ILogger, ExceptionReceivedContext, Exception> _serviceBusExceptionReceived = LoggerMessage.Define<ExceptionReceivedContext>(
            LogLevel.Error,
            EventIds.ServiceBusExceptionReceived,
            "Exception received from service bus (Context: {context})"
        );

        internal static void ServiceBusExceptionReceived(this ILogger logger, Exception exception, ExceptionReceivedContext exceptionReceivedContext)
            => _serviceBusExceptionReceived(logger, exceptionReceivedContext, exception);
        
        private static readonly Action<ILogger, string, Exception> _messagesProcessingThrew = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.MessagesProcessingThrew,
            "Exception threw while processing a message with id: {messageId}"
        );

        internal static void MessagesProcessingThrew(this ILogger logger, Exception exception, string messageId)
            => _messagesProcessingThrew(logger, messageId, exception);

        private static readonly Action<ILogger, string, Exception> _messageSent = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.MessageSent,
            "Sent a message with id: {messageId}"
        );

        
        internal static void NewSubscriptionCreated(this ILogger logger, string subscriptionName)
            => _newSubscriptionCreated(logger, subscriptionName, null);

        private static readonly Action<ILogger, string, Exception> _newSubscriptionCreated = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.NewSubscriptionCreated,
            "Created subscription with name: {subscriptionName}"
        );

        internal static void MessageSent(this ILogger logger, string messageId)
            => _messageSent(logger, messageId, null);

        internal static class EventIds
        {
            public static EventId ServiceBusExceptionReceived { get; } = new EventId(1, nameof(ServiceBusExceptionReceived));
            public static EventId MessagesProcessingThrew { get; } = new EventId(2, nameof(MessagesProcessingThrew));
            public static EventId MessageSent { get; } = new EventId(3, nameof(MessageSent));
            public static EventId NewSubscriptionCreated { get; } = new EventId(4, nameof(NewSubscriptionCreated));
        }
    }
}
