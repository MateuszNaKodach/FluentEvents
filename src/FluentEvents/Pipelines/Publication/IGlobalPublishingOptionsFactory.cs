using FluentEvents.Transmission;

namespace FluentEvents.Pipelines.Publication
{
    public interface IGlobalPublishingOptionsFactory
    {
        GlobalPublishingOptions With<T>() where T : IEventSender;
    }
}