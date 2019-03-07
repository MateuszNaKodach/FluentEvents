using System;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Azure.ServiceBus.Sending;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Sending
{
    [TestFixture]
    public class TopicClientFactoryTests
    {
        private const string ValidConnectionString = "Endpoint=sb://sbdomain.net/;SharedAccessKeyName=read;SharedAccessKey=123;EntityPath=123";

        private TopicClientFactory m_TopicClientFactory;

        [SetUp]
        public void SetUp()
        {
            m_TopicClientFactory = new TopicClientFactory();
        }

        [Test]
        public void GetNew_ShouldReturnTopicClient()
        {
            var topicClient = m_TopicClientFactory.GetNew(ValidConnectionString);

            Assert.That(topicClient, Is.TypeOf<TopicClient>());
        }
    }
}
