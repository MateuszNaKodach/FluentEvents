using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Pipelines.Publication;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests
{
    [TestFixture]
    public class ConfigureTransmissionExtensionsTests
    {
        private Mock<IConfigureTransmission> m_ConfigureTransmissionMock;

        [SetUp]
        public void SetUp()
        {
            m_ConfigureTransmissionMock = new Mock<IConfigureTransmission>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            m_ConfigureTransmissionMock.Verify();
        }

        [Test]
        public void WithAzureTopic_ShouldConfigureTransmissionWithAzureTopicEventSender()
        {
            var publishTransmissionConfiguration = new Mock<IPublishTransmissionConfiguration>(MockBehavior.Strict);

            m_ConfigureTransmissionMock
                .Setup(x => x.With<AzureTopicEventSender>())
                .Returns(publishTransmissionConfiguration.Object)
                .Verifiable();

            var returnedPublishTransmissionConfiguration = m_ConfigureTransmissionMock.Object.WithAzureTopic();

            Assert.That(publishTransmissionConfiguration.Object, Is.EqualTo(returnedPublishTransmissionConfiguration));
        }
    }
}
