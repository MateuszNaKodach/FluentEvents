using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Azure.ServiceBus
{
    public static class LoggerMessages
    {
        private static readonly Action<ILogger, ExceptionReceivedContext, Exception> m_ServiceBusExceptionReceived = LoggerMessage.Define<ExceptionReceivedContext>(
            LogLevel.Error,
            EventIds.ServiceBusExceptionReceived,
            "Exception received from service bus (Context: {context})"
        );

        public static void ServiceBusExceptionReceived(this ILogger logger, Exception exception, ExceptionReceivedContext exceptionReceivedContext)
            => m_ServiceBusExceptionReceived(logger, exceptionReceivedContext, exception);
        
        private static readonly Action<ILogger, string, Exception> m_MessagesProcessingThrew = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.MessagesProcessingThrew,
            "Exception threw while processing a message with id: {messageId}"
        );

        public static void MessagesProcessingThrew(this ILogger logger, Exception exception, string messageId)
            => m_MessagesProcessingThrew(logger, messageId, exception);

        private static readonly Action<ILogger, string, Exception> m_MessageSent = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.MessageSent,
            "Sent a message with id: {messageId}"
        );

        public static void MessageSent(this ILogger logger, string messageId)
            => m_MessageSent(logger, messageId, null);

        public sealed class EventIds
        {
            public static EventId ServiceBusExceptionReceived { get; } = new EventId(1, nameof(ServiceBusExceptionReceived));
            public static EventId MessagesProcessingThrew { get; } = new EventId(2, nameof(MessagesProcessingThrew));
            public static EventId MessageSent { get; } = new EventId(3, nameof(MessageSent));
        }
    }
}
