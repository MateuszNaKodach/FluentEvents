namespace FluentEvents.Pipelines.Publication
{
    /// <inheritdoc />
    public class ConfigureTransmission : IConfigureTransmission
    {
        IPublishTransmissionConfiguration IConfigureTransmission.With<T>() => new PublishTransmissionConfiguration(typeof(T));

        internal PublishTransmissionConfiguration Locally() => new PublishTransmissionConfiguration(null);
    }
}