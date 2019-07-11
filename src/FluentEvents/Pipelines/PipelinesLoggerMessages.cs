using System;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Pipelines
{
    internal static class PipelinesLoggerMessages
    {
        private static readonly Action<ILogger, string, string, Exception> _invokingPipelineModule = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            EventIds.InvokingPipelineModule,
            "Invoking pipeline module {pipelineModuleTypeName} for event of type: {eventType}"
        );

        internal static void InvokingPipelineModule(this ILogger logger, Type pipelineModuleType, PipelineEvent pipelineEvent)
            => _invokingPipelineModule(
                logger,
                pipelineModuleType.Name,
                pipelineEvent.Event.GetType().Name,
                null
            );

        internal static class EventIds
        {
            public static EventId InvokingPipelineModule { get; } = new EventId(1, nameof(InvokingPipelineModule));
        }
    }
}
