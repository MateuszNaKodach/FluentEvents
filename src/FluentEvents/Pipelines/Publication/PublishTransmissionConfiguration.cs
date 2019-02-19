using System;

namespace FluentEvents.Pipelines.Publication
{
    /// <inheritdoc />
    public class PublishTransmissionConfiguration : IPublishTransmissionConfiguration
    {
        private readonly Type m_SenderType;
        Type IPublishTransmissionConfiguration.SenderType => m_SenderType;

        internal PublishTransmissionConfiguration(Type senderType)
        {
            m_SenderType = senderType;
        }
    }
}