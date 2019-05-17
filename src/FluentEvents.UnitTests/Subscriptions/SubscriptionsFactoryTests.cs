using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionsFactoryTests
    {
        private Mock<ISourceModelsService> _sourceModelsServiceMock;
        private Mock<ISubscriptionScanService> _subscriptionScanServiceMock;
        private SourceModel _sourceModel;

        private SubscriptionsFactory _subscriptionsFactory;

        [SetUp]
        public void SetUp()
        {
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _subscriptionScanServiceMock = new Mock<ISubscriptionScanService>(MockBehavior.Strict);
            _sourceModel = new SourceModel(typeof(EventsSource));
            _sourceModel.GetOrCreateEventField(nameof(EventsSource.TestEvent));

            _subscriptionsFactory = new SubscriptionsFactory(
                _sourceModelsServiceMock.Object,
                _subscriptionScanServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _sourceModelsServiceMock.Verify();
            _subscriptionScanServiceMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldScanSubscribedHandlersAndReturnNewSubscription()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(EventsSource)))
                .Returns(_sourceModel)
                .Verifiable();

            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(
                    _sourceModel.ClrType, 
                    It.Is<IEnumerable<FieldInfo>>(y => y.SequenceEqual(_sourceModel.EventFields.Select(z => z.FieldInfo))), 
                    It.IsAny<Action<object>>())
                )
                .Returns(new List<SubscribedHandler>())
                .Verifiable();

            _subscriptionsFactory.CreateSubscription<EventsSource>(x => { });
        }

        private class EventsSource
        {
            public event EventHandler TestEvent;
        }
    }
}


