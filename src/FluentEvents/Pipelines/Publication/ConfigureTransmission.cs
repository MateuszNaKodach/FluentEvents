namespace FluentEvents.Pipelines.Publication
{
    public class ConfigureTransmission : IConfigureTransmission
    {
        PublishTransmissionConfiguration IConfigureTransmission.With<T>() => new PublishTransmissionConfiguration(typeof(T));

        internal PublishTransmissionConfiguration Locally() => new PublishTransmissionConfiguration(null);
    }
}