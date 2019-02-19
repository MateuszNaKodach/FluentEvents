using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     Provides a simple API surface to specify the sender to invoke for the transmission being configured. 
    /// </summary>
    public interface IConfigureTransmission
    {
        /// <summary>
        ///     This method creates a <see cref="PublishTransmissionConfiguration"/> with the specified sender type.
        /// </summary>
        /// <typeparam name="T">The type of the event sender.</typeparam>
        /// <returns>A configuration object with the specified sender type.</returns>
        PublishTransmissionConfiguration With<T>() where T : IEventSender;
    }
}