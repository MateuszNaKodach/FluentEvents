using System;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionsFactoryTests
    {
        private Mock<IInfrastructureEventsContext> m_EventsContextMock;
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private SourceModel m_SourceModel;

        private SubscriptionsFactory m_SubscriptionsFactory;

        [SetUp]
        public void SetUp()
        {
            m_EventsContextMock = new Mock<IInfrastructureEventsContext>(MockBehavior.Strict);
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(EventsSource), m_EventsContextMock.Object);
            m_SourceModel.GetOrCreateEventField(nameof(EventsSource.TestEvent));

            m_SubscriptionsFactory = new SubscriptionsFactory(m_SourceModelsServiceMock.Object);
        }

        [Test]
        public async Task CreateSubscription_ShouldTrackHandlersAndReturnNewSubscription()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(EventsSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            var isEventHandled = false;
            var subscription = m_SubscriptionsFactory.CreateSubscription<EventsSource>(x =>
            {
                x.TestEvent += (sender, args) => { isEventHandled = true; };
            });

            await subscription.PublishEventAsync(
                new PipelineEvent(nameof(EventsSource.TestEvent), new EventsSource(), EventArgs.Empty)
            );

            Assert.That(subscription, Is.Not.Null);
            Assert.That(isEventHandled, Is.True);
        }

        private class EventsSource
        {
            public event EventHandler TestEvent;
        }
    }
}


