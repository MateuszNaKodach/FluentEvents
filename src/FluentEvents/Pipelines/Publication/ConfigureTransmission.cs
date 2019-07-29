namespace FluentEvents.Pipelines.Publication
{
    /// <summary>
    ///     Provides a simple API surface to specify the sender to invoke for the transmission being configured. 
    /// </summary>
    public sealed class ConfigureTransmission : IConfigureTransmission
    {
        IPublishTransmissionConfiguration IConfigureTransmission.With<T>() => new PublishTransmissionConfiguration(typeof(T));

        internal static PublishTransmissionConfiguration Locally() => new PublishTransmissionConfiguration(null);
    }
}