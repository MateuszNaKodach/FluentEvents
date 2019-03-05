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
        private Mock<EventsContext> m_EventsContextMock1;
        private Mock<EventsContext> m_EventsContextMock2;
        private Mock<IAppServiceProvider> m_AppServiceProviderMock;
        private Mock<IInternalServiceCollection> m_InternalServiceCollectionMock1;
        private Mock<IInternalServiceCollection> m_InternalServiceCollectionMock2;
        private Mock<IServiceProvider> m_InternalServiceProviderMock1;
        private Mock<IServiceProvider> m_InternalServiceProviderMock2;
        private Mock<IScopedSubscriptionsService> m_ScopedSubscriptionsServiceMock1;
        private Mock<IScopedSubscriptionsService> m_ScopedSubscriptionsServiceMock2;

        private EventsContext[] m_EventsContexts;
        private EventsScope m_EventsScope;
        private Mock<IEventsQueueCollection> m_EventsQueueCollectionMock;

        [SetUp]
        public void SetUp()
        {
            m_EventsContextMock1 = new Mock<EventsContext>();
            m_EventsContextMock2 = new Mock<EventsContext>();
            m_EventsQueueCollectionMock = new Mock<IEventsQueueCollection>(MockBehavior.Strict);
            m_AppServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            m_InternalServiceCollectionMock1 = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            m_InternalServiceCollectionMock2 = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            m_InternalServiceProviderMock1 = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_InternalServiceProviderMock2 = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_ScopedSubscriptionsServiceMock1 = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            m_ScopedSubscriptionsServiceMock2 = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            
            SetUpEventsContext(m_EventsContextMock1, m_InternalServiceCollectionMock1, m_InternalServiceProviderMock1);
            SetUpEventsContext(m_EventsContextMock2, m_InternalServiceCollectionMock2, m_InternalServiceProviderMock2);

            m_EventsContexts = new[]
            {
                m_EventsContextMock1.Object,
                m_EventsContextMock2.Object
            };

            m_EventsScope = new EventsScope(
                m_EventsContexts,
                m_AppServiceProviderMock.Object,
                m_EventsQueueCollectionMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_EventsContextMock1.Verify();
            m_EventsContextMock2.Verify();
            m_EventsQueueCollectionMock.Verify();
            m_AppServiceProviderMock.Verify();
            m_InternalServiceCollectionMock1.Verify();
            m_InternalServiceCollectionMock2.Verify();
            m_InternalServiceProviderMock1.Verify();
            m_InternalServiceProviderMock2.Verify();
            m_ScopedSubscriptionsServiceMock1.Verify();
            m_ScopedSubscriptionsServiceMock2.Verify();
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
                .Returns(new SubscriptionsBuilder(eventsContextMock.Object, serviceProviderMock.Object, null, null))
                .Verifiable();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(PipelinesBuilder)))
                .Returns(new PipelinesBuilder(eventsContextMock.Object, serviceProviderMock.Object, null))
                .Verifiable();

            eventsContextMock.Object.Configure(new EventsContextOptions(), internalServiceCollectionMock.Object);
        }

        [Test]
        public void GetSubscriptions_OnFirstCall_ShouldCreateSubscriptionsForEveryContext()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = m_EventsScope.GetSubscriptions().ToArray();

            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
        }

        [Test]
        public void GetSubscriptions_OnSecondCall_ShouldNotCreateSubscriptions()
        {
            var allSubscriptions = SetUpSubscriptionsCreation().ToArray();

            var createdSubscriptions = m_EventsScope.GetSubscriptions().ToArray();

            var storedSubscriptions = m_EventsScope.GetSubscriptions().ToArray();
            
            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
            Assert.That(storedSubscriptions, Is.EquivalentTo(createdSubscriptions));
        }
        
        private IEnumerable<Subscription> SetUpSubscriptionsCreation()
        {
            var scopedSubscriptionsFactory1Subscriptions = new[] { new Subscription(typeof(object)), new Subscription(typeof(object)) };
            var scopedSubscriptionsFactory2Subscriptions = new[] { new Subscription(typeof(object)) };
            var allSubscriptions = scopedSubscriptionsFactory1Subscriptions.Concat(scopedSubscriptionsFactory2Subscriptions);
            
            m_InternalServiceProviderMock1
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(m_ScopedSubscriptionsServiceMock1.Object)
                .Verifiable();

            m_ScopedSubscriptionsServiceMock1
                .Setup(x => x.SubscribeServices(m_AppServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory1Subscriptions)
                .Verifiable();
            
            m_InternalServiceProviderMock2
                .Setup(x => x.GetService(typeof(IScopedSubscriptionsService)))
                .Returns(m_ScopedSubscriptionsServiceMock2.Object)
                .Verifiable();

            m_ScopedSubscriptionsServiceMock2
                .Setup(x => x.SubscribeServices(m_AppServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory2Subscriptions)
                .Verifiable();

            return allSubscriptions;
        }
    }
}
