using System;

namespace FluentEvents.Pipelines.Publication
{
    public class GlobalPublishingOptions : IPublishTransmissionConfiguration
    {
        private readonly Type m_SenderType;
        Type IPublishTransmissionConfiguration.SenderType => m_SenderType;

        internal GlobalPublishingOptions(Type senderType)
        {
            m_SenderType = senderType;
        }
    }
}