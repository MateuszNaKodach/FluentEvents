using System;

namespace FluentEvents.Pipelines.Publication
{
    internal class PublishTransmissionConfiguration : IPublishTransmissionConfiguration
    {
        private readonly Type _senderType;
        Type IPublishTransmissionConfiguration.SenderType => _senderType;

        public PublishTransmissionConfiguration(Type senderType)
        {
            _senderType = senderType;
        }
    }
}