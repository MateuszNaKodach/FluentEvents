using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Infrastructure
{
    [TestFixture]
    public class EventsScopeTests
    {
        private Mock<IScopedAppServiceProvider> _scopedAppServiceProviderMock;
        private Mock<IServiceProvider> _internalServiceProviderMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;

        private EventsScope _eventsScope;

        [SetUp]
        public void SetUp()
        {
            _scopedAppServiceProviderMock = new Mock<IScopedAppServiceProvider>(MockBehavior.Strict);
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            _eventsScope = new EventsScope(_internalServiceProviderMock.Object, _scopedAppServiceProviderMock.Object);
        }

        [Test]
        public void GetSubscriptions_OnFirstCall_ShouldCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScope.GetSubscriptions().ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
        }

        [Test]
        public void GetSubscriptions_OnSecondCall_ShouldNotCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = _eventsScope.GetSubscriptions().ToArray();

            var storedSubscriptions = _eventsScope.GetSubscriptions().ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
            Assert.That(storedSubscriptions, Is.EquivalentTo(createdSubscriptions));
        }

        [Test]
        public void GetOrAddEventsQueue_WithExistingQueue_ShouldOnlyReturn()
        {
            var eventsQueue1 = _eventsScope.GetOrAddEventsQueue("1");
            var eventsQueue2 = _eventsScope.GetOrAddEventsQueue("1");

            var eventsQueues = _eventsScope.GetEventsQueues();

            Assert.That(eventsQueues, Has.One.Items);
            Assert.That(eventsQueue1, Is.EqualTo(eventsQueue2));
        }

        [Test]
        public void GetEventsQueues_ShouldReturnAllQueues()
        {
            _eventsScope.GetOrAddEventsQueue("1");
            _eventsScope.GetOrAddEventsQueue("2");

            var eventsQueues = _eventsScope.GetEventsQueues();

            Assert.That(eventsQueues, Has.Exactly(2).Items);
        }

        private IEnumerable<Subscription> SetUpSubscriptionsCreation()
        {
            Action<object> handler = e => { };

            var scopedSubscriptions = new[]
            {
                new Subscription(typeof(object), handler),
                new Subscription(typeof(object), handler)
            };

            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(_scopedSubscriptionsServiceMock.Object)
                .Verifiable();

            _scopedSubscriptionsServiceMock
                .Setup(x => x.SubscribeServices(_scopedAppServiceProviderMock.Object))
                .Returns(scopedSubscriptions)
                .Verifiable();

            return scopedSubscriptions;
        }
    }

}
