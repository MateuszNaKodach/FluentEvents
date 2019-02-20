using FluentEvents.Pipelines;

namespace FluentEvents.Transmission
{
    /// <summary>
    ///     A service that serializes or deserializes an event for transmission.
    /// </summary>
    public interface IEventsSerializationService
    {
        /// <summary>
        ///     This method should serialize an event.
        /// </summary>
        /// <param name="pipelineEvent">The event to serialize.</param>
        /// <returns>The event serialized.</returns>
        byte[] SerializeEvent(PipelineEvent pipelineEvent);

        /// <summary>
        ///     This method should deserialize an event.
        /// </summary>
        /// <param name="eventData">The event to deserialize.</param>
        /// <returns>The event deserialized.</returns>
        PipelineEvent DeserializeEvent(byte[] eventData);
    }
}