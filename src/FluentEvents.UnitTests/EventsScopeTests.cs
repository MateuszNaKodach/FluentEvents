using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Queues;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class EventsScopeTests
    {
        private Mock<IEventsContext> m_EventsContextMock;
        private Mock<IScopedSubscriptionsFactory> m_ScopedSubscriptionsFactoryMock1;
        private Mock<IScopedSubscriptionsFactory> m_ScopedSubscriptionsFactoryMock2;
        private IScopedSubscriptionsFactory[] m_ScopedSubscriptionsFactories;
        private Mock<IServiceProvider> m_AppServiceProviderMock;
        private Mock<IServiceProvider> m_InternalServiceProviderMock;
        private Mock<IEventsQueuesService> m_EventsQueuesServiceMock;
        private EventsScope m_EventsScope;

        [SetUp]
        public void SetUp()
        {
            m_EventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);
            m_ScopedSubscriptionsFactoryMock1 = new Mock<IScopedSubscriptionsFactory>(MockBehavior.Strict);
            m_ScopedSubscriptionsFactoryMock2 = new Mock<IScopedSubscriptionsFactory>(MockBehavior.Strict);
            m_ScopedSubscriptionsFactories = new[]
            {
                m_ScopedSubscriptionsFactoryMock1.Object,
                m_ScopedSubscriptionsFactoryMock2.Object
            };
            m_AppServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_EventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);
            m_EventsScope = new EventsScope(
                m_ScopedSubscriptionsFactories, 
                m_AppServiceProviderMock.Object,
                m_EventsQueuesServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_ScopedSubscriptionsFactoryMock1.Verify();
            m_ScopedSubscriptionsFactoryMock2.Verify();
            m_AppServiceProviderMock.Verify();
            m_EventsQueuesServiceMock.Verify();
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

            Assert.That(m_ScopedSubscriptionsFactoryMock1.Invocations, Has.One.Items);
            Assert.That(m_ScopedSubscriptionsFactoryMock2.Invocations, Has.One.Items);
            Assert.That(createdSubscriptions, Is.EquivalentTo(allSubscriptions));
            Assert.That(storedSubscriptions, Is.EquivalentTo(createdSubscriptions));
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_ShouldCallEventsQueuesService()
        {
            const string queueName = "queueName";

            m_EventsQueuesServiceMock
                .Setup(x => x.ProcessQueuedEventsAsync(m_EventsScope, m_EventsContextMock.Object, queueName))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_EventsScope.ProcessQueuedEventsAsync(m_EventsContextMock.Object, queueName);
        }

        [Test]
        public void DiscardQueuedEvents_ShouldCallEventsQueuesService()
        {
            const string queueName = "queueName";

            m_EventsQueuesServiceMock
                .Setup(x => x.DiscardQueuedEvents(m_EventsContextMock.Object, queueName))
                .Verifiable();

            m_EventsScope.DiscardQueuedEvents(m_EventsContextMock.Object, queueName);
        }

        [Test]
        public void EnqueueEvent_ShouldCallEventsQueuesService()
        {
            const string queueName = "queueName";
            var pipelineEvent = new PipelineEvent("a", new object(), new object());
            var pipeline = new Pipeline(queueName, null, m_InternalServiceProviderMock.Object);

            m_EventsQueuesServiceMock
                .Setup(x => x.EnqueueEvent(pipelineEvent, pipeline))
                .Verifiable();

            m_EventsScope.EnqueueEvent(pipelineEvent, pipeline);
        }

        private IEnumerable<Subscription> SetUpSubscriptionsCreation()
        {
            var scopedSubscriptionsFactory1Subscriptions = new[] { new Subscription(typeof(object)), new Subscription(typeof(object)) };
            var scopedSubscriptionsFactory2Subscriptions = new[] { new Subscription(typeof(object)) };
            var allSubscriptions = scopedSubscriptionsFactory1Subscriptions.Concat(scopedSubscriptionsFactory2Subscriptions);

            m_ScopedSubscriptionsFactoryMock1
                .Setup(x => x.CreateScopedSubscriptionsForServices(m_AppServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory1Subscriptions)
                .Verifiable();

            m_ScopedSubscriptionsFactoryMock2
                .Setup(x => x.CreateScopedSubscriptionsForServices(m_AppServiceProviderMock.Object))
                .Returns(scopedSubscriptionsFactory2Subscriptions)
                .Verifiable();

            return allSubscriptions;
        }
    }
}
