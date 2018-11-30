namespace FluentEvents.Pipelines
{
    public class PipelineEvent
    {
        public string OriginalEventFieldName { get; }
        public object OriginalSender { get; }
        public object OriginalEventArgs { get; }

        public PipelineEvent(string originalEventFieldName, object originalSender, object originalEventArgs)
        {
            OriginalEventFieldName = originalEventFieldName;
            OriginalSender = originalSender;
            OriginalEventArgs = originalEventArgs;
        }
    }
}