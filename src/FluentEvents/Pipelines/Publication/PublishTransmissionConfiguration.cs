using System;

namespace FluentEvents.Pipelines.Publication
{
    /// <inheritdoc />
    public class PublishTransmissionConfiguration : IPublishTransmissionConfiguration
    {
        private readonly Type _senderType;
        Type IPublishTransmissionConfiguration.SenderType => _senderType;

        internal PublishTransmissionConfiguration(Type senderType)
        {
            _senderType = senderType;
        }
    }
}