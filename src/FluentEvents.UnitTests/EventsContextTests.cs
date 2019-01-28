using System;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class EventsContextTests
    {
        private EventsContextImpl m_EventsContext;
        private EventsContextOptions m_EventsContextOptions;
        private Mock<IInternalServiceCollection> m_InternalServiceCollectionMock;
        private Mock<IServiceProvider> m_InternalServiceProviderMock;
        private Mock<IScopedSubscriptionsService> m_ScopedSubscriptionsServiceMock;
        private Mock<IGlobalSubscriptionCollection> m_GlobalSubscriptionCollectionMock;
        private Mock<IEventReceiversService> m_EventReceiversServiceMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<IAttachingService> m_AttachingServiceMock;
        private EventsContextDependencies m_EventsContextDependencies;

        private Mock<EventsScope> m_EventsScopeMock;
        private Mock<IEventsQueuesService> m_EventsQueuesServiceMock;

        [SetUp]
        public void SetUp()
        {
            m_EventsContext = new EventsContextImpl();
            m_EventsContextOptions = new EventsContextOptions();
            m_InternalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            m_InternalServiceCollectionMock = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            m_ScopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            m_GlobalSubscriptionCollectionMock = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            m_EventReceiversServiceMock = new Mock<IEventReceiversService>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_AttachingServiceMock = new Mock<IAttachingService>(MockBehavior.Strict);
            m_EventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);

            m_EventsScopeMock = new Mock<EventsScope>(MockBehavior.Strict);

            m_EventsContextDependencies = new EventsContextDependencies(
                m_GlobalSubscriptionCollectionMock.Object,
                m_EventReceiversServiceMock.Object,
                m_AttachingServiceMock.Object,
                m_EventsQueuesServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_InternalServiceProviderMock.Verify();
            m_InternalServiceCollectionMock.Verify();
            m_ScopedSubscriptionsServiceMock.Verify();
            m_GlobalSubscriptionCollectionMock.Verify();
            m_EventReceiversServiceMock.Verify();
            m_SourceModelsServiceMock.Verify();
            m_AttachingServiceMock.Verify();
            m_EventsQueuesServiceMock.Verify();
        }

        [Test]
        public void Configure_ShouldBuildServiceProvider()
        {
            SetUpServiceProviderAndServiceCollection();
          
            m_EventsContext.Configure(m_EventsContextOptions, m_InternalServiceCollectionMock.Object);
        }

        [Test]
        public void Build_ShouldCallBuilders()
        {
            SetUpServiceProviderAndServiceCollection();
            m_EventsContext.Configure(m_EventsContextOptions, m_InternalServiceCollectionMock.Object);
            SetUpGetBuilders();

            var isOnBuildingPipelinesCalled = false;
            m_EventsContext.OnBuildingPipelinesCalled += (sender, args) =>
            {
                isOnBuildingPipelinesCalled = true;
            };

            m_EventsContext.Build();

            Assert.That(isOnBuildingPipelinesCalled, Is.True);
        }

        [Test]
        public void StartEventReceivers_ShouldCallEventReceiversService()
        {
            ConfigureEventsContext();

            var cts = new CancellationTokenSource();

            m_EventReceiversServiceMock
                .Setup(x => x.StartReceiversAsync(cts.Token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventsContext.StartEventReceivers(cts.Token);
        }

        [Test]
        public void StopEventReceivers_ShouldCallEventReceiversService()
        {
            ConfigureEventsContext();

            var cts = new CancellationTokenSource();

            m_EventReceiversServiceMock
                .Setup(x => x.StopReceiversAsync(cts.Token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            m_EventsContext.StopEventReceivers(cts.Token);
        }

        [Test]
        public void Attach_ShouldCallAttachingService()
        {
            ConfigureEventsContext();

            var source = new object();

            m_AttachingServiceMock
                .Setup(x => x.Attach(source, m_EventsScopeMock.Object))
                .Verifiable();

            m_EventsContext.Attach(source, m_EventsScopeMock.Object);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_ShouldCallEventsQueuesServiceProcessQueuedEventsAsync()
        {
            ConfigureEventsContext();

            const string queueName = "queueName";

            m_EventsQueuesServiceMock
                .Setup(x => x.ProcessQueuedEventsAsync(m_EventsScopeMock.Object, queueName))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await m_EventsContext.ProcessQueuedEventsAsync(m_EventsScopeMock.Object, queueName);
        }

        [Test]
        public void DiscardQueuedEventsAsync_ShouldCallEventsQueuesServiceDiscardQueuedEventsAsync()
        {
            ConfigureEventsContext();

            const string queueName = "queueName";

            m_EventsQueuesServiceMock
                .Setup(x => x.DiscardQueuedEvents(m_EventsScopeMock.Object, queueName))
                .Verifiable();

            m_EventsContext.DiscardQueuedEvents(m_EventsScopeMock.Object, queueName);
        }

        [Test]
        public void MakeGlobalSubscriptionTo_ShouldAddToGlobalSubscriptionCollection()
        {
            ConfigureEventsContext();

            Action<object> subscriptionAction = o => { };
            var subscription = new Subscription(typeof(object));

            m_GlobalSubscriptionCollectionMock
                .Setup(x => x.AddGlobalScopeSubscription(subscriptionAction))
                .Returns(subscription)
                .Verifiable();

            var returnedSubscription = m_EventsContext.MakeGlobalSubscriptionTo(subscriptionAction);

            Assert.That(returnedSubscription, Is.EqualTo(subscription));
        }

        [Test]
        public void CancelGlobalSubscription_ShouldRemoveFromGlobalSubscriptionCollection()
        {
            ConfigureEventsContext();

            var subscription = new Subscription(typeof(object));

            m_GlobalSubscriptionCollectionMock
                .Setup(x => x.RemoveGlobalScopeSubscription(subscription))
                .Verifiable();

            m_EventsContext.CancelGlobalSubscription(subscription);
        }
        
        private void ConfigureEventsContext()
        {
            SetUpServiceProviderAndServiceCollection();
            m_EventsContext.Configure(m_EventsContextOptions, m_InternalServiceCollectionMock.Object);

            SetUpGetBuilders();
            m_EventsContext.Build();
        }

        private void SetUpServiceProviderAndServiceCollection()
        {
            m_InternalServiceCollectionMock
                .Setup(x => x.BuildServiceProvider(m_EventsContext, m_EventsContextOptions))
                .Returns(m_InternalServiceProviderMock.Object)
                .Verifiable();

            m_InternalServiceProviderMock
                .Setup(x => x.GetService(typeof(IEventsContextDependencies)))
                .Returns(m_EventsContextDependencies)
                .Verifiable();
        }

        private void SetUpGetBuilders()
        {
            m_InternalServiceProviderMock
                .Setup(x => x.GetService(typeof(SubscriptionsBuilder)))
                .Returns(new SubscriptionsBuilder(
                    m_EventsContext,
                    m_InternalServiceProviderMock.Object,
                    m_GlobalSubscriptionCollectionMock.Object,
                    m_ScopedSubscriptionsServiceMock.Object
                ))
                .Verifiable();

            m_InternalServiceProviderMock
                .Setup(x => x.GetService(typeof(PipelinesBuilder)))
                .Returns(new PipelinesBuilder(
                    m_EventsContext,
                    m_InternalServiceProviderMock.Object,
                    m_SourceModelsServiceMock.Object
                ))
                .Verifiable();
        }

        private class EventsContextImpl : EventsContext
        {
            public event EventHandler OnBuildingPipelinesCalled;

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                OnBuildingPipelinesCalled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
