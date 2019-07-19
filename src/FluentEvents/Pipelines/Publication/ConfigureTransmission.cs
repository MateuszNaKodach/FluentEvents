namespace FluentEvents.Pipelines.Publication
{
    /// <inheritdoc />
    public sealed class ConfigureTransmission : IConfigureTransmission
    {
        IPublishTransmissionConfiguration IConfigureTransmission.With<T>() => new PublishTransmissionConfiguration(typeof(T));

        internal static PublishTransmissionConfiguration Locally() => new PublishTransmissionConfiguration(null);
    }
}