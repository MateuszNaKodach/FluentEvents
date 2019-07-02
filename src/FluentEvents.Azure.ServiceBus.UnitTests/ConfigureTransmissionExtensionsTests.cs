using FluentEvents.Azure.ServiceBus.Common;
using FluentEvents.Pipelines.Publication;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests
{
    [TestFixture]
    public class ConfigureTransmissionExtensionsTests
    {
        private Mock<IConfigureTransmission> _configureTransmissionMock;

        [SetUp]
        public void SetUp()
        {
            _configureTransmissionMock = new Mock<IConfigureTransmission>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _configureTransmissionMock.Verify();
        }

        [Test]
        public void WithAzureTopic_ShouldConfigureTransmissionWithAzureTopicEventSender()
        {
            var publishTransmissionConfiguration = new Mock<IPublishTransmissionConfiguration>(MockBehavior.Strict);

            _configureTransmissionMock
                .Setup(x => x.With<AzureServiceBusEventSenderBase>())
                .Returns(publishTransmissionConfiguration.Object)
                .Verifiable();

            var returnedPublishTransmissionConfiguration = _configureTransmissionMock.Object.WithAzureTopic();

            Assert.That(publishTransmissionConfiguration.Object, Is.EqualTo(returnedPublishTransmissionConfiguration));
        }
    }
}
