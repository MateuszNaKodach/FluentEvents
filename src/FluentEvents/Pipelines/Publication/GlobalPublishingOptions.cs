using System;

namespace FluentEvents.Pipelines.Publication
{
    public class GlobalPublishingOptions : ISenderTypeConfiguration
    {
        private readonly Type m_SenderType;
        Type ISenderTypeConfiguration.SenderType => m_SenderType;

        internal GlobalPublishingOptions(Type senderType)
        {
            m_SenderType = senderType;
        }
    }
}