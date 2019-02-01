using System;

namespace FluentEvents.Pipelines
{
    public class PipelineEvent
    {
        public Type OriginalSenderType { get; }
        public string OriginalEventFieldName { get; }
        public object OriginalSender { get; }
        public object OriginalEventArgs { get; }

        public PipelineEvent(
            Type originalSenderType,
            string originalEventFieldName, 
            object originalSender,
            object originalEventArgs
        )
        {
            OriginalSenderType = originalSenderType;
            OriginalEventFieldName = originalEventFieldName;
            OriginalSender = originalSender;
            OriginalEventArgs = originalEventArgs;
        }
    }
}