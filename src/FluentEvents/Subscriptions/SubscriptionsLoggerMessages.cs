using System;
using FluentEvents.Pipelines;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Subscriptions
{
    internal static class SubscriptionsLoggerMessages
    {
        private static readonly Action<ILogger, Exception> m_EventHandlerThrew = LoggerMessage.Define(
            LogLevel.Error,
            TransmissionLoggerMessages.EventIds.EventReceiverStarting,
            "An event handler threw an exception"
        );

        internal static void EventHandlerThrew(this ILogger logger, Exception exception)
            => m_EventHandlerThrew(logger, exception);

        private static readonly Action<ILogger, string, string, Exception> m_PublishingEvent = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            EventIds.PublishingEvent,
            "Publishing event fired from {eventSenderTypeName}.{eventSenderFieldName}"
        );

        internal static void PublishingEvent(this ILogger logger, PipelineEvent pipelineEvent)
            => m_PublishingEvent(
                logger,
                pipelineEvent.OriginalSender.GetType().Name,
                pipelineEvent.OriginalEventFieldName,
                null
            );

        internal sealed class EventIds
        {
            public static EventId EventHandlerThrew { get; } = new EventId(1, nameof(EventHandlerThrew));
            public static EventId PublishingEvent { get; } = new EventId(2, nameof(PublishingEvent));
        }
    }
}
