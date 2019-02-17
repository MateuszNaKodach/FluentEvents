using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    public interface IConfigureTransmission
    {
        PublishTransmissionConfiguration With<T>() where T : IEventSender;
    }
}