using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class TopicEventReceiverTests
    {
        private TopicEventReceiverConfig m_Config;

        private Mock<ILogger<TopicEventReceiver>> m_LoggerMock;
        private Mock<IPublishingService> m_PublishingServiceMock;
        private Mock<IEventsSerializationService> m_EventsSerializationServiceMock;
        private Mock<ITopicSubscriptionsService> m_TopicSubscriptionsServiceMock;
        private Mock<ISubscriptionClientFactory> m_SubscriptionClientFactoryMock;

        private TopicEventReceiver m_TopicEventReceiver;

        [SetUp]
        public void SetUp()
        {
            m_Config = new TopicEventReceiverConfig();
            m_LoggerMock = new Mock<ILogger<TopicEventReceiver>>(MockBehavior.Strict);
            m_PublishingServiceMock = new Mock<IPublishingService>(MockBehavior.Strict);
            m_EventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            m_TopicSubscriptionsServiceMock = new Mock<ITopicSubscriptionsService>(MockBehavior.Strict);
            m_SubscriptionClientFactoryMock = new Mock<ISubscriptionClientFactory>(MockBehavior.Strict);

            m_TopicEventReceiver = new TopicEventReceiver(
                m_LoggerMock.Object,
                Options.Create(m_Config),
                m_PublishingServiceMock.Object,
                m_EventsSerializationServiceMock.Object,
                m_TopicSubscriptionsServiceMock.Object,
                m_SubscriptionClientFactoryMock.Object
            );
        }
    }
}
