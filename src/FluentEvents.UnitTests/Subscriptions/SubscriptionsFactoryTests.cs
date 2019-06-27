using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        private SubscribedHandler _subscribedHandler;

        private SubscriptionsFactory _subscriptionsFactory;

        [SetUp]
        public void SetUp()
        {
            _sourceModelsServiceMock = new Mock<ISourceModelsService>(MockBehavior.Strict);
            _subscriptionScanServiceMock = new Mock<ISubscriptionScanService>(MockBehavior.Strict);

            _sourceModel = new SourceModel(typeof(EventsSource));
            _sourceModel.GetOrCreateEventField(nameof(EventsSource.TestEvent));

            var service = new SubscribingService();
            Func<object, object, Task> action = service.HandleEventAsync;
            var handler = Delegate.CreateDelegate(action.GetType(), service, action.Method);
            _subscribedHandler = new SubscribedHandler("", handler);

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
        public void CreateSubscription_WithSubscribedHandler_ShouldReturnNewSubscription()
        {
            var subscription = _subscriptionsFactory.CreateSubscription<EventsSource>(_subscribedHandler);

            Assert.That(subscription, Is.Not.Null);
        }

        [Test]
        public void CreateSubscription_WithSubscriptionAction_ShouldScanSubscribedHandlersAndReturnNewSubscription()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(EventsSource)))
                .Returns(_sourceModel)
                .Verifiable();

            _subscriptionScanServiceMock
                .Setup(x => x.GetSubscribedHandlers(
                    _sourceModel, 
                    It.IsAny<Action<object>>())
                )
                .Callback<SourceModel, Action<object>>((_, action) => action(new EventsSource()))
                .Returns(new [] { _subscribedHandler })
                .Verifiable();

            var subscription = _subscriptionsFactory.CreateSubscription<EventsSource>(x => { });

            Assert.That(subscription, Is.Not.Null);
        }

        [Test]
        public void CreateSubscription_WithSubscriptionActionAndSourceModelNotConfigured_ShouldThrow()
        {
            _sourceModelsServiceMock
                .Setup(x => x.GetSourceModel(typeof(EventsSource)))
                .Returns<SourceModel>(null)
                .Verifiable();

            Assert.That(
                () => _subscriptionsFactory.CreateSubscription<EventsSource>(x => { }),
                Throws.TypeOf<SourceIsNotConfiguredException>()
            );
        }

        private class SubscribingService : IEventHandler<object, object>
        {
            public Task HandleEventAsync(object source, object args)
            {
                throw new NotImplementedException();
            }
        }

        private class EventsSource
        {
            public event EventHandler TestEvent;
        }
    }
}


