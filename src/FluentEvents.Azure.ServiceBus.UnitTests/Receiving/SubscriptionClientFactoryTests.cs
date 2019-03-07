using System;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Azure.ServiceBus.Receiving;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Receiving
{
    [TestFixture]
    public class SubscriptionClientFactoryTests
    {
        private const string ReceiveConnectionString = "Endpoint=sb://sb.net/;SharedAccessKeyName=read;SharedAccessKey=0;EntityPath=0";
        private const string SubscriptionName = nameof(SubscriptionName);

        private SubscriptionClientFactory m_SubscriptionClientFactory;

        [SetUp]
        public void SetUp()
        {
            m_SubscriptionClientFactory = new SubscriptionClientFactory();
        }

        [Test]
        public void GetNew_ShouldReturnSubscriptionClient()
        {
            var subscriptionClient = m_SubscriptionClientFactory.GetNew(ReceiveConnectionString, SubscriptionName);

            Assert.That(subscriptionClient, Is.TypeOf<SubscriptionClient>());
            Assert.That(
                subscriptionClient,
                Has.Property(nameof(SubscriptionClient.SubscriptionName)).EqualTo(SubscriptionName)
            );
        }
    }
}
