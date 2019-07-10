using System;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Azure.ServiceBus.Queues.Common;
using FluentEvents.Azure.ServiceBus.Topics.Sending;
using FluentEvents.Azure.ServiceBus.UnitTests.Common;
using FluentEvents.Transmission;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Topics.Sending
{
    [TestFixture]
    public class AzureServiceBusTopicEventSenderTests
    {
        private const string ConnectionString = Constants.ValidConnectionString;

        private Mock<ILogger<AzureServiceBusTopicEventSender>> _loggerMock;
        private Mock<IEventsSerializationService> _eventsSerializationServiceMock;
        private Mock<ITopicClientFactory> _topicClientFactoryMock;
        private Mock<ITopicClient> _topicClientMock;
        private AzureServiceBusTopicEventSenderConfig _azureServiceBusTopicEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<AzureServiceBusTopicEventSender>>(MockBehavior.Strict);
            _eventsSerializationServiceMock = new Mock<IEventsSerializationService>(MockBehavior.Strict);
            _topicClientFactoryMock = new Mock<ITopicClientFactory>(MockBehavior.Strict);
            _topicClientMock = new Mock<ITopicClient>(MockBehavior.Strict);
            _azureServiceBusTopicEventSenderConfig = new AzureServiceBusTopicEventSenderConfig
            {
                SendConnectionString = ConnectionString
            };
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _eventsSerializationServiceMock.Verify();
            _topicClientFactoryMock.Verify();
            _topicClientMock.Verify();
        }

        [Test]
        public void Ctor_ShouldGetTopicClientFromFactory()
        {
            _topicClientFactoryMock
                .Setup(x => x.GetNew(ConnectionString))
                .Returns(_topicClientMock.Object)
                .Verifiable();

            var azureServiceBusTopicEventSender = new AzureServiceBusTopicEventSender(
                _loggerMock.Object,
                _eventsSerializationServiceMock.Object,
                Options.Create(_azureServiceBusTopicEventSenderConfig),
                _topicClientFactoryMock.Object
            );
        }
    }
}
