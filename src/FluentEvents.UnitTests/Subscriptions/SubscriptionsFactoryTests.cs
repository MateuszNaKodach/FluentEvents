using System;
using System.Collections.Generic;
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
        private Mock<ISourceModelsService> m_SourceModelsServiceMock;
        private Mock<ISubscriptionScanService> m_SubscriptionScanServiceMock;
        private SourceModel m_SourceModel;

        private SubscriptionsFactory m_SubscriptionsFactory;

        [SetUp]
        public void SetUp()
        {
            m_SourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            m_SubscriptionScanServiceMock = new Mock<ISubscriptionScanService>(MockBehavior.Strict);
            m_SourceModel = new SourceModel(typeof(EventsSource));
            m_SourceModel.GetOrCreateEventField(nameof(EventsSource.TestEvent));

            m_SubscriptionsFactory = new SubscriptionsFactory(
                m_SourceModelsServiceMock.Object,
                m_SubscriptionScanServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_SourceModelsServiceMock.Verify();
            m_SubscriptionScanServiceMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldScanSubscribedHandlersAndReturnNewSubscription()
        {
            m_SourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(EventsSource)))
                .Returns(m_SourceModel)
                .Verifiable();

            Action<object> subscriptionAction = x => { };

            m_SubscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(m_SourceModel, subscriptionAction))
                .Returns(new List<SubscribedHandler>())
                .Verifiable();

            m_SubscriptionsFactory.CreateSubscription<EventsSource>(subscriptionAction);
        }

        private class EventsSource
        {
            public event EventHandler TestEvent;
        }
    }
}


