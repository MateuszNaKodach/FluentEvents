using System;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Transmission
{
    internal static class TransmissionLoggerMessages
    {
        private static readonly Action<ILogger, string, Exception> m_EventReceiverStarting = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStarting,
            "Starting {eventReceiverTypeName} event receiver"
        );

        internal static void EventReceiverStarting(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStarting(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStarted = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStarted,
            "Started {eventReceiverTypeName} event receiver"
        );

        internal static void EventReceiverStarted(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStarted(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStopping = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStopping,
            "Stopping {eventReceiverTypeName} event receiver"
        );

        internal static void EventReceiverStopping(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStopping(logger, eventReceiverTypeName, null);

        private static readonly Action<ILogger, string, Exception> m_EventReceiverStopped = LoggerMessage.Define<string>(
            LogLevel.Information,
            EventIds.EventReceiverStopped,
            "Stopped {eventReceiverTypeName} event receiver"
        );

        internal static void EventReceiverStopped(this ILogger logger, string eventReceiverTypeName)
            => m_EventReceiverStopped(logger, eventReceiverTypeName, null);

        internal static class EventIds
        {
            internal static EventId EventReceiverStarting { get; } = new EventId(1, nameof(EventReceiverStarting));
            internal static EventId EventReceiverStarted { get; } = new EventId(2, nameof(EventReceiverStarted));
            internal static EventId EventReceiverStopping { get; } = new EventId(3, nameof(EventReceiverStopping));
            internal static EventId EventReceiverStopped { get; } = new EventId(4, nameof(EventReceiverStopped));
        }
    }
}
