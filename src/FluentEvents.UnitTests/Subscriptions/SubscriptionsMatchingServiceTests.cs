using System;
using FluentEvents.Subscriptions;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionsMatchingServiceTests
    {
        private SubscriptionsMatchingService _subscriptionsMatchingService;

        [SetUp]
        public void SetUp()
        {
            _subscriptionsMatchingService = new SubscriptionsMatchingService();
        }

        [Test]
        public void GetMatchingSubscriptionsForSender_ShouldFilterSubscriptionsBasedOnEventTypeAndInheritanceAndInterfaces()
        {
            Action<object> handler = e => { };
            var subscription0 = new Subscription(typeof(TestEvent1), handler);
            var subscription1 = new Subscription(typeof(TestEvent2), handler);
            var subscription3 = new Subscription(typeof(TestEvent3), handler);
            var subscription4 = new Subscription(typeof(ITestEvent), handler);

            var subscriptions = new[]
            {
                subscription0,
                subscription1,
                subscription3,
                subscription4
            };

            var testEvent2 = new TestEvent2();

            var matchingSubscriptions = _subscriptionsMatchingService
                .GetMatchingSubscriptionsForEvent(subscriptions, testEvent2);

            Assert.That(matchingSubscriptions, Is.EquivalentTo(new []
            {
                subscription0,
                subscription1,
                subscription4
            }));
        }

        private class TestEvent1
        {
        }

        private class TestEvent2 : TestEvent1, ITestEvent
        {
        }

        private class TestEvent3
        {
        }
        private interface ITestEvent
        {
        }
    }
}
