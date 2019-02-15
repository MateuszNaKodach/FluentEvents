using System;

namespace FluentEvents.Pipelines.Publication
{
    public interface IPublishTransmissionConfiguration
    {
        Type SenderType { get; }
    }
}