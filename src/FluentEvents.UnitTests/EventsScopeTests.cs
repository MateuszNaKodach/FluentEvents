using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class EventsScopeTests
    {
        private Mock<EventsContext> _eventsContextMock1;
        private Mock<EventsContext> _eventsContextMock2;
        private Mock<IAppServiceProvider> _appServiceProviderMock;
        private Mock<IInternalServiceCollection> _internalServiceCollectionMock1;
        private Mock<IInternalServiceCollection> _internalServiceCollectionMock2;
        private Mock<IServiceProvider> _internalServiceProviderMock1;
        private Mock<IServiceProvider> _internalServiceProviderMock2;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock1;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock2;

        private EventsContext[] _eventsContexts;
        private EventsScope _eventsScope;
        private Mock<IEventsQueueCollection> _eventsQueueCollectionMock;

        [SetUp]
        public void SetUp()
        {
            _eventsContextMock1 = new Mock<EventsContext>();
            _eventsContextMock2 = new Mock<EventsContext>();
            _eventsQueueCollectionMock = new Mock<IEventsQueueCollection>(MockBehavior.Strict);
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            _internalServiceCollectionMock1 = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            _internalServiceCollectionMock2 = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            _internalServiceProviderMock1 = new Mock<IServiceProvider>(MockBehavior.Strict);
            _internalServiceProviderMock2 = new Mock<IServiceProvider>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock1 = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock2 = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            
            SetUpEventsContext(_eventsContextMock1, _internalServiceCollectionMock1, _internalServiceProviderMock1);
            SetUpEventsContext(_eventsContextMock2, _internalServiceCollectionMock2, _internalServiceProviderMock2);

            _eventsContexts = new[]
            {
                _eventsContextMock1.Object,
                _eventsContextMock2.Object
            };

            _eventsScope = new EventsScope(
                _eventsContexts,
                _appServiceProviderMock.Object,
                _eventsQueueCollectionMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _eventsContextMock1.Verify();
            _eventsContextMock2.Verify();
            _eventsQueueCollectionMock.Verify();
            _appServiceProviderMock.Verify();
            _internalServiceCollectionMock1.Verify();
            _internalServiceCollectionMock2.Verify();
            _internalServiceProviderMock1.Verify();
            _internalServiceProviderMock2.Verify();
            _scopedSubscriptionsServiceMock1.Verify();
            _scopedSubscriptionsServiceMock2.Verify();
        }

        private static void SetUpEventsContext(
            Mock<EventsContext> eventsContextMock, 
            Mock<IInternalServiceCollection> internalServiceCollectionMock,
            Mock<IServiceProvider> serviceProviderMock
        )
        {
            internalServiceCollectionMock
                .Setup(x => x.BuildServiceProvider(eventsContextMock.Object, It.IsAny<IFluentEventsPluginOptions>()))
                .Returns(serviceProviderMock.Object)
                .Verifiable();
            
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IValidableConfig>)))
                .Returns(new IValidableConfig[0])
                .Verifiable();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(SubscriptionsBuilder)))
                .Returns(new SubscriptionsBuilder(null, null))
                .Verifiable();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(PipelinesBuilder)))
                .Returns(new PipelinesBuilder(serviceProviderMock.Object, null, null))
                .Verifiable();

            eventsContextMock.Object.Configure(new EventsContextOptions(), internalServiceCollectionMock.Object);
        }

        [Test]
        public void GetSubscriptions_OnFirstCall_ShouldCreateSubscriptionsForEveryContext()
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
        
        private IEnumerable<Subscription> SetUpSubscriptionsCreation()
        {
            var scopedSubscriptionsFactory1Subscriptions = new[] { new Subscription(typeof(object)), new Subscription(typeof(object)) };
            var scopedSubscriptionsFactory2Subscriptions = new[] { new Subscription(typeof(object)) };
            var allSubscriptions = scopedSubscriptionsFactory1Subscriptions.Concat(scopedSubscriptionsFactory2Subscriptions);
            
            _internalServiceProviderMock1
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(_scopedSubscriptionsServiceMock1.Object)
                .Verifiable();

            _scopedSubscriptionsServiceMock1
                .Setup(x => x.SubscribeServices(_appServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory1Subscriptions)
                .Verifiable();
            
            _internalServiceProviderMock2
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(_scopedSubscriptionsServiceMock2.Object)
                .Verifiable();

            _scopedSubscriptionsServiceMock2
                .Setup(x => x.SubscribeServices(_appServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory2Subscriptions)
                .Verifiable();

            return allSubscriptions;
        }
    }
}
