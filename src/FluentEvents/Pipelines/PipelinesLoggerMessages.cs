using System;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Pipelines
{
    internal static class PipelinesLoggerMessages
    {
        private static readonly Action<ILogger, string, string, string, Exception> m_InvokingPipelineModule = LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            EventIds.InvokingPipelineModule,
            "Invoking pipeline module {pipelineModuleTypeName} for event fired from {eventSenderTypeName}.{eventSenderFieldName}"
        );

        internal static void InvokingPipelineModule(this ILogger logger, Type pipelineModuleType, PipelineEvent pipelineEvent)
            => m_InvokingPipelineModule(
                logger,
                pipelineModuleType.Name,
                pipelineEvent.OriginalSender.GetType().Name,
                pipelineEvent.OriginalEventFieldName,
                null
            );

        internal static class EventIds
        {
            public static EventId InvokingPipelineModule { get; } = new EventId(1, nameof(InvokingPipelineModule));
        }
    }
}
