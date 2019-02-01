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
        private Mock<ILogger<PublishingService>> m_LoggerMock;
        private Mock<IGlobalSubscriptionCollection> m_GlobalSubscriptionCollectionMock;
        private Mock<ISubscriptionsMatchingService> m_SubscriptionsMatchingServiceMock;
        private Mock<EventsScope> m_EventsScopeMock;

        private PublishingService m_PublishingService;
        private Subscription[] m_Subscriptions;
        private PipelineEvent m_PipelineEvent;

        [SetUp]
        public void SetUp()
        {
            m_LoggerMock = new Mock<ILogger<PublishingService>>(MockBehavior.Strict);
            m_GlobalSubscriptionCollectionMock = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            m_SubscriptionsMatchingServiceMock = new Mock<ISubscriptionsMatchingService>(MockBehavior.Strict);
            m_EventsScopeMock = new Mock<EventsScope>(MockBehavior.Strict);

            m_PublishingService = new PublishingService(
                m_LoggerMock.Object,
                m_GlobalSubscriptionCollectionMock.Object,
                m_SubscriptionsMatchingServiceMock.Object
            );

            m_Subscriptions = new[]
            {
                new Subscription(typeof(object)),
                new Subscription(typeof(object)),
                new Subscription(typeof(object)),
            };

            m_PipelineEvent = new PipelineEvent(
                typeof(object), 
                "fieldName", 
                new object(),
                new object()
            );

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
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
            m_LoggerMock.Verify();
            m_GlobalSubscriptionCollectionMock.Verify();
            m_SubscriptionsMatchingServiceMock.Verify();
            m_EventsScopeMock.Verify();
        }

        [Test]
        public async Task PublishEventToScopedSubscriptionsAsync_ShouldHandlePublishingException()
        {
            SetUpEventsScopeGetSubscriptions();
            SetUpSubscriptionsMatchingService();

            var exception = new Exception();
            SetUpSubscriptionEventsHandler(m_Subscriptions[0], (sender, args) => throw exception);
            SetUpSubscriptionEventsHandler(m_Subscriptions[1], (sender, args) => throw exception);

            m_LoggerMock
                .Setup(x => x.IsEnabled(LogLevel.Error))
                .Returns(true)
                .Verifiable();

            m_LoggerMock
                .Setup(x => x.Log(
                    LogLevel.Error,
                    SubscriptionsLoggerMessages.EventIds.EventHandlerThrew,
                    It.IsAny<object>(),
                    exception,
                    It.IsAny<Func<object, Exception, string>>()
                ))
                .Verifiable();

            await m_PublishingService.PublishEventToScopedSubscriptionsAsync(
                m_PipelineEvent,
                m_EventsScopeMock.Object
            );
        }

        [Test]
        public async Task PublishEventToScopedSubscriptionsAsync_ShouldGetSubscriptionsFromScopeAndPublishToMatchingSubscriptions()
        {
            SetUpEventsScopeGetSubscriptions();

            await TestPublishing(async () =>
            {
                await m_PublishingService.PublishEventToScopedSubscriptionsAsync(
                    m_PipelineEvent,
                    m_EventsScopeMock.Object
                );
            });
        }

        private void SetUpEventsScopeGetSubscriptions()
        {
            m_EventsScopeMock
                .Setup(x => x.GetSubscriptions())
                .Returns(m_Subscriptions)
                .Verifiable();
        }

        [Test]
        public async Task PublishEventToGlobalSubscriptionsAsync_ShouldGetSubscriptionsFromGlobalScopeAndPublishToMatchingSubscriptions()
        {
            m_GlobalSubscriptionCollectionMock
                .Setup(x => x.GetGlobalScopeSubscriptions())
                .Returns(m_Subscriptions)
                .Verifiable();

            await TestPublishing(async () =>
            {
                await m_PublishingService.PublishEventToGlobalSubscriptionsAsync(
                    m_PipelineEvent
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

            SetUpSubscriptionEventsHandler(m_Subscriptions[0], Subscription0HandlerAction);
            SetUpSubscriptionEventsHandler(m_Subscriptions[1], Subscription1HandlerAction);

            await testAction();

            Assert.That(subscription0Sender, Is.EqualTo(m_PipelineEvent.OriginalSender));
            Assert.That(subscription0Args, Is.EqualTo(m_PipelineEvent.OriginalEventArgs));
            Assert.That(subscription1Sender, Is.EqualTo(m_PipelineEvent.OriginalSender));
            Assert.That(subscription1Args, Is.EqualTo(m_PipelineEvent.OriginalEventArgs));
        }

        private void SetUpSubscriptionsMatchingService()
        {
            m_SubscriptionsMatchingServiceMock
                .Setup(x => x.GetMatchingSubscriptionsForSender(m_Subscriptions, m_PipelineEvent.OriginalSender))
                .Returns(new[] {m_Subscriptions[0], m_Subscriptions[1]})
                .Verifiable();
        }

        private void SetUpSubscriptionEventsHandler(Subscription subscription, Action<object, object> handlerAction)
        {
            subscription.AddHandler(m_PipelineEvent.OriginalEventFieldName, handlerAction.GetInvocationList()[0]);
        }
    }
}
