using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class PublishingServiceTests
    {
        private readonly Exception _exception = new Exception();

        private Mock<ILogger<PublishingService>> _loggerMock;
        private Mock<IGlobalSubscriptionsService> _globalSubscriptionsServiceMock;
        private Mock<ISubscriptionsMatchingService> _subscriptionsMatchingServiceMock;
        private Mock<IEventsContext> _eventsContextMock;
        private Mock<IEventsScope> _eventsScopeMock;
        private Mock<IEventsScopeSubscriptionsFeature> _eventsScopeSubscriptionsFeatureMock;

        private PublishingService _publishingService;
        private Subscription[] _subscriptions;
        private PipelineEvent _pipelineEvent;

        private object _subscription1Event;
        private object _subscription0Event;
        private bool _isThrowingEnabled;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<PublishingService>>(MockBehavior.Strict);
            _globalSubscriptionsServiceMock = new Mock<IGlobalSubscriptionsService>(MockBehavior.Strict);
            _subscriptionsMatchingServiceMock = new Mock<ISubscriptionsMatchingService>(MockBehavior.Strict);
            _eventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);
            _eventsScopeMock = new Mock<IEventsScope>(MockBehavior.Strict);
            _eventsScopeSubscriptionsFeatureMock = new Mock<IEventsScopeSubscriptionsFeature>(MockBehavior.Strict);

            _publishingService = new PublishingService(
                _loggerMock.Object,
                _eventsContextMock.Object,
                _globalSubscriptionsServiceMock.Object,
                _subscriptionsMatchingServiceMock.Object
            );

            Action<object> subscription0HandlerAction = args =>
            {
                ThrowIfEnabled();
                _subscription0Event = args;
            };

            Action<object> subscription1HandlerAction = args =>
            {
                ThrowIfEnabled();
                _subscription1Event = args;
            };

            _subscriptions = new[]
            {
                new Subscription(typeof(object), subscription0HandlerAction.GetInvocationList()[0]),
                new Subscription(typeof(object), subscription1HandlerAction.GetInvocationList()[0]),
            };

            _pipelineEvent = new PipelineEvent(typeof(object));

            _subscription1Event = null;
            _subscription0Event = null;
            _isThrowingEnabled = false;
        }   

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _globalSubscriptionsServiceMock.Verify();
            _subscriptionsMatchingServiceMock.Verify();
            _eventsContextMock.Verify();
            _eventsScopeMock.Verify();
            _eventsScopeSubscriptionsFeatureMock.Verify();
        }

        [Test]
        public void PublishEventToScopedSubscriptionsAsync_WithNullEventsScope_ShouldThrow()
        {
            Assert.That(async () =>
            {
                await _publishingService.PublishEventToScopedSubscriptionsAsync(
                    _pipelineEvent,
                    null
                );
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PublishEventToScopedSubscriptionsAsync_ShouldAggregateAndLogPublishingException()
        {
            SetUpLogger();
            SetUpEventsScopeGetSubscriptions();
            SetUpSubscriptionsMatchingService();

            _isThrowingEnabled = true;

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    SubscriptionsLoggerMessages.EventIds.EventHandlerThrew,
                    It.IsAny<object>(),
                    _exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            Assert.That(async () =>
            {
                await _publishingService.PublishEventToScopedSubscriptionsAsync(
                    _pipelineEvent,
                    _eventsScopeMock.Object
                );
            }, Throws.TypeOf<SubscriptionPublishAggregateException>());
        }

        [Test]
        public async Task PublishEventToScopedSubscriptionsAsync_ShouldGetSubscriptionsFromScopeAndPublishToMatchingSubscriptions()
        {
            SetUpLogger();
            SetUpEventsScopeGetSubscriptions();

            await TestPublishing(async () =>
            {
                await _publishingService.PublishEventToScopedSubscriptionsAsync(
                    _pipelineEvent,
                    _eventsScopeMock.Object
                );
            });
        }

        [Test]
        public async Task PublishEventToGlobalSubscriptionsAsync_ShouldGetSubscriptionsFromGlobalScopeAndPublishToMatchingSubscriptions()
        {
            SetUpLogger();
            _globalSubscriptionsServiceMock
                .Setup(x => x.GetGlobalSubscriptions())
                .Returns(_subscriptions)
                .Verifiable();

            await TestPublishing(async () =>
            {
                await _publishingService.PublishEventToGlobalSubscriptionsAsync(
                    _pipelineEvent
                );
            });
        }

        private void SetUpLogger()
        {
            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Information,
                    SubscriptionsLoggerMessages.EventIds.PublishingEvent,
                    It.IsAny<object>(),
                    null,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();
        }

        private void SetUpEventsScopeGetSubscriptions()
        {
            _eventsScopeMock
                .Setup(x => x.GetOrAddFeature(It.IsAny<Func<IScopedAppServiceProvider, IEventsScopeSubscriptionsFeature>>()))
                .Returns(_eventsScopeSubscriptionsFeatureMock.Object)
                .Verifiable();

            _eventsScopeSubscriptionsFeatureMock
                .Setup(x => x.GetSubscriptions(_eventsContextMock.Object))
                .Returns(_subscriptions)
                .Verifiable();
        }

        private async Task TestPublishing(Func<Task> testAction)
        {
            SetUpSubscriptionsMatchingService();
            
            await testAction();

            Assert.That(_subscription0Event, Is.EqualTo(_pipelineEvent.Event));
            Assert.That(_subscription1Event, Is.EqualTo(_pipelineEvent.Event));
        }

        private void SetUpSubscriptionsMatchingService()
        {
            _subscriptionsMatchingServiceMock
                .Setup(x => x.GetMatchingSubscriptionsForEvent(_subscriptions, _pipelineEvent.Event))
                .Returns(_subscriptions)
                .Verifiable();
        }
        private void ThrowIfEnabled()
        {
            if (_isThrowingEnabled)
                throw _exception;
        }
    }
}
