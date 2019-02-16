using System;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Routing
{
    public static class RoutingLoggerMessages
    {
        private static readonly Func<ILogger, string, string, IDisposable> m_BeginEventRoutingScope = LoggerMessage.DefineScope<string, string>(
            "Routing event fired from {eventSenderTypeName}.{eventSenderFieldName}"
        );

        public static IDisposable BeginEventRoutingScope(this ILogger logger, PipelineEvent pipelineEvent)
            => m_BeginEventRoutingScope(logger, pipelineEvent.OriginalSender.GetType().Name, pipelineEvent.OriginalEventFieldName);

        private static readonly Action<ILogger, Exception> m_EventRoutedToPipeline = LoggerMessage.Define(
            LogLevel.Information,
            EventIds.EventRoutedToPipeline,
            "Routing event to pipeline"
        );

        public static void EventRoutedToPipeline(this ILogger logger)
            => m_EventRoutedToPipeline(logger, null);

        public sealed class EventIds
        {
            public static EventId EventRoutedToPipeline { get; } = new EventId(1, nameof(EventRoutedToPipeline));
        }
    }
}
