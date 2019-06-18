using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;
using FluentEvents.Utils;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
{
    [TestFixture]
    public class EventsContextTests
    {
        private EventsContextImpl _eventsContext;
        private EventsContextOptions _eventsContextOptions;
        private Mock<IInternalServiceCollection> _internalServiceCollectionMock;
        private Mock<IServiceProvider> _internalServiceProviderMock;
        private Mock<IScopedSubscriptionsService> _scopedSubscriptionsServiceMock;
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<IEventReceiversService> _eventReceiversServiceMock;
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<IAttachingService> _attachingServiceMock;
        private EventsContextDependencies _eventsContextDependencies;

        private Mock<EventsScope> _eventsScopeMock;
        private Mock<IEventsQueuesService> _eventsQueuesServiceMock;
        private Mock<IValidableConfig> _validableConfigMock;
        private Mock<IEventSelectionService> _eventSelectionServiceMock;

        [SetUp]
        public void SetUp()
        {
            _eventsContext = new EventsContextImpl();
            _eventsContextOptions = new EventsContextOptions();
            _internalServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            _internalServiceCollectionMock = new Mock<IInternalServiceCollection>(MockBehavior.Strict);
            _scopedSubscriptionsServiceMock = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _eventReceiversServiceMock = new Mock<IEventReceiversService>(MockBehavior.Strict);
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _attachingServiceMock = new Mock<IAttachingService>(MockBehavior.Strict);
            _eventsQueuesServiceMock = new Mock<IEventsQueuesService>(MockBehavior.Strict);
            _validableConfigMock = new Mock<IValidableConfig>(MockBehavior.Strict);
            _eventSelectionServiceMock = new Mock<IEventSelectionService>(MockBehavior.Strict);

            _eventsScopeMock = new Mock<EventsScope>(MockBehavior.Strict);

            _eventsContextDependencies = new EventsContextDependencies(
                _globalSubscriptionsServiceMock.Object,
                _eventReceiversServiceMock.Object,
                _attachingServiceMock.Object,
                _eventsQueuesServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _internalServiceProviderMock.Verify();
            _internalServiceCollectionMock.Verify();
            _scopedSubscriptionsServiceMock.Verify();
            _globalSubscriptionsServiceMock.Verify();
            _eventReceiversServiceMock.Verify();
            _sourceModelsServiceMock.Verify();
            _attachingServiceMock.Verify();
            _eventsQueuesServiceMock.Verify();
            _validableConfigMock.Verify();
            _eventSelectionServiceMock.Verify();
        }

        [Test]
        public void Configure_ShouldOverrideOptionsAndServiceProvider()
        {
            SetUpServiceProviderAndServiceCollection();
            SetUpBuilding();

            _eventsContext.Configure(_eventsContextOptions, _internalServiceCollectionMock.Object);

            var serviceProvider = _eventsContext.Get<IServiceProvider>();

            Assert.That(serviceProvider, Is.EqualTo(_internalServiceProviderMock.Object));
        }

        [Test]
        [Sequential]
        public void Configure_WithNullArgs_ShouldThrow(
            [Values(false, true)] bool areOptionsNull, 
            [Values(true, false)] bool isServiceCollectionNull
        )
        {
            Assert.That(() =>
            {
                _eventsContext.Configure(
                    areOptionsNull ? null : _eventsContextOptions,
                    isServiceCollectionNull ? null : _internalServiceCollectionMock.Object
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Instance_OnFirstCall_ShouldCallBuilders()
        {
            SetUpServiceProviderAndServiceCollection();
            _eventsContext.Configure(_eventsContextOptions, _internalServiceCollectionMock.Object);
            SetUpBuilding();

            var isOnBuildingPipelinesCalled = false;
            _eventsContext.OnBuildingPipelinesCalled += (sender, args) =>
            {
                isOnBuildingPipelinesCalled = true;
            };

            _eventsContext.Get<IServiceProvider>();

            Assert.That(isOnBuildingPipelinesCalled, Is.True);
        }

        [Test]
        public async Task StartEventReceiversAsync_ShouldCallEventReceiversService()
        {
            ConfigureEventsContext();

            var cts = new CancellationTokenSource();

            _eventReceiversServiceMock
                .Setup(x => x.StartReceiversAsync(cts.Token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _eventsContext.StartEventReceiversAsync(cts.Token);
        }

        [Test]
        public async Task StopEventReceiversAsync_ShouldCallEventReceiversService()
        {
            ConfigureEventsContext();

            var cts = new CancellationTokenSource();

            _eventReceiversServiceMock
                .Setup(x => x.StopReceiversAsync(cts.Token))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _eventsContext.StopEventReceiversAsync(cts.Token);
        }

        [Test]
        public void Attach_ShouldCallAttachingService()
        {
            ConfigureEventsContext();

            var source = new object();

            _attachingServiceMock
                .Setup(x => x.Attach(source, _eventsScopeMock.Object))
                .Verifiable();

            _eventsContext.Attach(source, _eventsScopeMock.Object);
        }

        [Test]
        public async Task ProcessQueuedEventsAsync_ShouldCallEventsQueuesServiceProcessQueuedEventsAsync()
        {
            ConfigureEventsContext();

            const string queueName = "queueName";

            _eventsQueuesServiceMock
                .Setup(x => x.ProcessQueuedEventsAsync(_eventsScopeMock.Object, queueName))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _eventsContext.ProcessQueuedEventsAsync(_eventsScopeMock.Object, queueName);
        }

        [Test]
        public void DiscardQueuedEventsAsync_ShouldCallEventsQueuesServiceDiscardQueuedEventsAsync()
        {
            ConfigureEventsContext();

            const string queueName = "queueName";

            _eventsQueuesServiceMock
                .Setup(x => x.DiscardQueuedEvents(_eventsScopeMock.Object, queueName))
                .Verifiable();

            _eventsContext.DiscardQueuedEvents(_eventsScopeMock.Object, queueName);
        }

        [Test]
        public void MakeGlobalSubscriptionTo_ShouldAddToGlobalSubscriptionCollection()
        {
            ConfigureEventsContext();

            Action<object> subscriptionAction = o => { };
            var subscription = new Subscription(typeof(object));

            _globalSubscriptionsServiceMock
                .Setup(x => x.AddGlobalSubscription(subscriptionAction))
                .Returns(subscription)
                .Verifiable();

            var returnedSubscription = _eventsContext.SubscribeGloballyTo(subscriptionAction);

            Assert.That(
                returnedSubscription,
                Has.Property(nameof(UnsubscribeToken.Subscription)).EqualTo(subscription)
            );
        }

        [Test]
        public void Unsubscribe_ShouldRemoveFromGlobalSubscriptionCollection()
        {
            ConfigureEventsContext();

            var subscription = new Subscription(typeof(object));

            _globalSubscriptionsServiceMock
                .Setup(x => x.RemoveGlobalSubscription(subscription))
                .Verifiable();

            _eventsContext.Unsubscribe(new UnsubscribeToken(subscription));
        }
        
        private void ConfigureEventsContext()
        {
            SetUpServiceProviderAndServiceCollection();
            SetUpGetDependencies();
            _eventsContext.Configure(_eventsContextOptions, _internalServiceCollectionMock.Object);

            SetUpBuilding();
        }

        private void SetUpServiceProviderAndServiceCollection()
        {
            _internalServiceCollectionMock
                .Setup(x => x.BuildServiceProvider(_eventsContext, _eventsContextOptions))
                .Returns(_internalServiceProviderMock.Object)
                .Verifiable();

        }

        private void SetUpGetDependencies()
        {
            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(IEventsContextDependencies)))
                .Returns(_eventsContextDependencies)
                .Verifiable();
        }

        private void SetUpBuilding()
        {
            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(SubscriptionsBuilder)))
                .Returns(new SubscriptionsBuilder(
                    _globalSubscriptionsServiceMock.Object,
                    _scopedSubscriptionsServiceMock.Object,
                    _sourceModelsServiceMock.Object,
                    _eventSelectionServiceMock.Object
                ))
                .Verifiable();

            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(PipelinesBuilder)))
                .Returns(new PipelinesBuilder(
                    _internalServiceProviderMock.Object,
                    _sourceModelsServiceMock.Object,
                    _eventSelectionServiceMock.Object
                ))
                .Verifiable();

            _validableConfigMock
                .Setup(x => x.Validate())
                .Verifiable();

            _internalServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IValidableConfig>)))
                .Returns(new [] { _validableConfigMock.Object })
                .Verifiable();
        }

        private class EventsContextImpl : EventsContext
        {
            public event EventHandler OnBuildingPipelinesCalled;

            public EventsContextImpl() : base(new EventsContextOptions())
            {
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                OnBuildingPipelinesCalled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
