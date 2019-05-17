using System;
using FluentEvents.Pipelines;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Routing
{
    internal static class RoutingLoggerMessages
    {
        private static readonly Func<ILogger, string, string, IDisposable> _beginEventRoutingScope = LoggerMessage.DefineScope<string, string>(
            "Routing event fired from {eventSenderTypeName}.{eventSenderFieldName}"
        );

        internal static IDisposable BeginEventRoutingScope(this ILogger logger, PipelineEvent pipelineEvent)
            => _beginEventRoutingScope(logger, pipelineEvent.OriginalSender.GetType().Name, pipelineEvent.OriginalEventFieldName);

        private static readonly Action<ILogger, Exception> _eventRoutedToPipeline = LoggerMessage.Define(
            LogLevel.Information,
            EventIds.EventRoutedToPipeline,
            "Routing event to pipeline"
        );

        internal static void EventRoutedToPipeline(this ILogger logger)
            => _eventRoutedToPipeline(logger, null);

        internal static class EventIds
        {
            public static EventId EventRoutedToPipeline { get; } = new EventId(1, nameof(EventRoutedToPipeline));
        }
    }
}
