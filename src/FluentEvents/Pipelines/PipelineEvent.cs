using System;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     Represents an event.
    /// </summary>
    public class PipelineEvent
    {
        /// <summary>
        ///     The type of the event source.
        /// </summary>
        public Type OriginalSenderType { get; }

        /// <summary>
        ///     The name of the event field.
        /// </summary>
        public string OriginalEventFieldName { get; }

        /// <summary>
        ///     The instance of the event source.
        /// </summary>
        public object OriginalSender { get; }

        /// <summary>
        ///     The instance of the event args.
        /// </summary>
        public object OriginalEventArgs { get; }

        /// <param name="originalSenderType">The type of the event source.</param>
        /// <param name="originalEventFieldName">The name of the event field.</param>
        /// <param name="originalSender">The instance of the event source.</param>
        /// <param name="originalEventArgs">The instance of the event args.</param>
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