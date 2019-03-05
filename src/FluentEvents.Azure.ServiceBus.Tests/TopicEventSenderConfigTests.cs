using System;
using FluentEvents.Azure.ServiceBus.Sending;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.Tests
{
    [TestFixture]
    public class TopicEventSenderConfigTests
    {
        private const string InvalidConnectionString = "InvalidConnectionString";
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private TopicEventSenderConfig m_TopicEventSenderConfig;

        [SetUp]
        public void SetUp()
        {
            m_TopicEventSenderConfig = new TopicEventSenderConfig();
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsInvalid_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_TopicEventSenderConfig.ConnectionString = InvalidConnectionString;
            }, Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void ConnectionString_WhenConnectionStringIsValid_ShouldSet()
        {
            m_TopicEventSenderConfig.ConnectionString = ValidConnectionString;

            Assert.That(
                m_TopicEventSenderConfig,
                Has
                    .Property(nameof(TopicEventSenderConfig.ConnectionString))
                    .EqualTo(ValidConnectionString)
            );
        }
    }
}
