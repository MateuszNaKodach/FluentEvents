﻿using FluentEvents.Subscriptions;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionsMatchingServiceTests
    {
        private SubscriptionsMatchingService m_SubscriptionsMatchingService;

        [SetUp]
        public void SetUp()
        {
            m_SubscriptionsMatchingService = new SubscriptionsMatchingService();
        }

        [Test]
        public void GetMatchingSubscriptionsForSender_ShouldFilterSubscriptionsBasedOnSenderTypeAndInheritance()
        {
            var subscription0 = new Subscription(typeof(TestSource1));
            var subscription1 = new Subscription(typeof(TestSource2));
            var subscription3 = new Subscription(typeof(TestSource3));

            var subscriptions = new[]
            {
                subscription0,
                subscription1,
                subscription3
            };

            var sender = new TestSource2();

            var matchingSubscriptions = m_SubscriptionsMatchingService
                .GetMatchingSubscriptionsForSender(subscriptions, sender);

            Assert.That(matchingSubscriptions, Is.EquivalentTo(new []
            {
                subscription0,
                subscription1
            }));
        }

        public class TestSource1
        {
        }

        public class TestSource2 : TestSource1
        {
        }

        public class TestSource3
        {
        }
    }
}