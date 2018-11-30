using System;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Transmission
{
    public static class TransmissionLoggerMessages
    {
        private static readonly Action<ILogger, string, Exception> m_EventReceiverStarting = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStarting,
            "Starting {eventReceiverTypeName} event receiver"
        );

        public static void EventReceiverStarting(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStarting(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStarted = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStarted,
            "Started {eventReceiverTypeName} event receiver"
        );

        public static void EventReceiverStarted(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStarted(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStopping = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStopping,
            "Stopping {eventReceiverTypeName} event receiver"
        );

        public static void EventReceiverStopping(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStopping(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStopped = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStopped,
            "Stopped {eventReceiverTypeName} event receiver"
        );

        public static void EventReceiverStopped(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStopped(logger, eventReceiverTypeName, null);

        public sealed class EventIds
        {
            public static EventId EventReceiverStarting { get; } = new EventId(1, nameof(EventReceiverStarting));
            public static EventId EventReceiverStarted { get; } = new EventId(2, nameof(EventReceiverStarted));
            public static EventId EventReceiverStopping { get; } = new EventId(3, nameof(EventReceiverStopping));
            public static EventId EventReceiverStopped { get; } = new EventId(4, nameof(EventReceiverStopped));
        }
    }
}
