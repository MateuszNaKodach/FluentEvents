using System;
using System.Threading.Tasks;
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
        private Mock<ILogger<PublishingService>> _loggerMock;
        private Mock<IGlobalSubscriptionCollection> _globalSubscriptionCollectionMock;
        private Mock<ISubscriptionsMatchingService> _subscriptionsMatchingServiceMock;
        private Mock<EventsScope> _eventsScopeMock;

        private PublishingService _publishingService;
        private Subscription[] _subscriptions;
        private PipelineEvent _pipelineEvent;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<PublishingService>>(MockBehavior.Strict);
            _globalSubscriptionCollectionMock = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            _subscriptionsMatchingServiceMock = new Mock<ISubscriptionsMatchingService>(MockBehavior.Strict);
            _eventsScopeMock = new Mock<EventsScope>(MockBehavior.Strict);

            _publishingService = new PublishingService(
                _loggerMock.Object,
                _globalSubscriptionCollectionMock.Object,
                _subscriptionsMatchingServiceMock.Object
            );

            _subscriptions = new[]
            {
                new Subscription(typeof(object)),
                new Subscription(typeof(object)),
                new Subscription(typeof(object)),
            };

            _pipelineEvent = new PipelineEvent(
                typeof(object), 
                "fieldName", 
                new object(),
                new object()
            );

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

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Verify();
            _globalSubscriptionCollectionMock.Verify();
            _subscriptionsMatchingServiceMock.Verify();
            _eventsScopeMock.Verify();
        }

        [Test]
        public async Task PublishEventToScopedSubscriptionsAsync_ShouldHandlePublishingException()
        {
            SetUpEventsScopeGetSubscriptions();
            SetUpSubscriptionsMatchingService();

            var exception = new Exception();
            SetUpSubscriptionEventsHandler(_subscriptions[0], (sender, args) => throw exception);
            SetUpSubscriptionEventsHandler(_subscriptions[1], (sender, args) => throw exception);

            _loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            _loggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    SubscriptionsLoggerMessages.EventIds.EventHandlerThrew,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await _publishingService.PublishEventToScopedSubscriptionsAsync(
                _pipelineEvent,
                _eventsScopeMock.Object
            );
        }

        [Test]
        public async Task PublishEventToScopedSubscriptionsAsync_ShouldGetSubscriptionsFromScopeAndPublishToMatchingSubscriptions()
        {
            SetUpEventsScopeGetSubscriptions();

            await TestPublishing(async () =>
            {
                await _publishingService.PublishEventToScopedSubscriptionsAsync(
                    _pipelineEvent,
                    _eventsScopeMock.Object
                );
            });
        }

        private void SetUpEventsScopeGetSubscriptions()
        {
            _eventsScopeMock
                .Setup(x => x.GetSubscriptions())
                .Returns(_subscriptions)
                .Verifiable();
        }

        [Test]
        public async Task PublishEventToGlobalSubscriptionsAsync_ShouldGetSubscriptionsFromGlobalScopeAndPublishToMatchingSubscriptions()
        {
            _globalSubscriptionCollectionMock
                .Setup(x => x.GetGlobalScopeSubscriptions())
                .Returns(_subscriptions)
                .Verifiable();

            await TestPublishing(async () =>
            {
                await _publishingService.PublishEventToGlobalSubscriptionsAsync(
                    _pipelineEvent
                );
            });
        }

        private async Task TestPublishing(Func<Task> testAction)
        {
            SetUpSubscriptionsMatchingService();

            object subscription0Sender = null;
            object subscription0Args = null;

            void Subscription0HandlerAction(object sender, object args)
            {
                subscription0Sender = sender;
                subscription0Args = args;
            }

            object subscription1Sender = null;
            object subscription1Args = null;

            void Subscription1HandlerAction(object sender, object args)
            {
                subscription1Sender = sender;
                subscription1Args = args;
            }

            SetUpSubscriptionEventsHandler(_subscriptions[0], Subscription0HandlerAction);
            SetUpSubscriptionEventsHandler(_subscriptions[1], Subscription1HandlerAction);

            await testAction();

            Assert.That(subscription0Sender, Is.EqualTo(_pipelineEvent.OriginalSender));
            Assert.That(subscription0Args, Is.EqualTo(_pipelineEvent.OriginalEventArgs));
            Assert.That(subscription1Sender, Is.EqualTo(_pipelineEvent.OriginalSender));
            Assert.That(subscription1Args, Is.EqualTo(_pipelineEvent.OriginalEventArgs));
        }

        private void SetUpSubscriptionsMatchingService()
        {
            _subscriptionsMatchingServiceMock
                .Setup(x => x.GetMatchingSubscriptionsForSender(_subscriptions, _pipelineEvent.OriginalSender))
                .Returns(new[] {_subscriptions[0], _subscriptions[1]})
                .Verifiable();
        }

        private void SetUpSubscriptionEventsHandler(Subscription subscription, Action<object, object> handlerAction)
        {
            subscription.AddHandler(_pipelineEvent.OriginalEventFieldName, handlerAction.GetInvocationList()[0]);
        }
    }
}
