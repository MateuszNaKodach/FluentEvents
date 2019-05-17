using System;
using System.Linq;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionScanServiceTests
    {
        private SubscriptionScanService _subscriptionScanService;
        private SourceModel _concreteSourceModel;
        private SourceModel _abstractSourceModel;

        [SetUp]
        public void SetUp()
        {
            _concreteSourceModel = new SourceModel(typeof(ConcreteSource));
            _concreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent1));
            _concreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent2));
            _concreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent3));

            _abstractSourceModel = new SourceModel(typeof(AbstractSource));
            _abstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent1));
            _abstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent2));
            _abstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent3));

            _subscriptionScanService = new SubscriptionScanService();
        }

        [Test]
        public void GetSubscribedHandlers_WithConcreteType_ShouldReturnEventHandlersWithSubscriptions()
        {
            void SubscriptionAction(ConcreteSource x)
            {
                x.TestEvent1 += delegate { };
                x.TestEvent1 += delegate { };
                x.TestEvent2 += delegate { };
            }

            var subscribedHandlers = _subscriptionScanService.GetSubscribedHandlers(
                _concreteSourceModel.ClrType,
                _concreteSourceModel.EventFields.Select(x => x.FieldInfo),
                x => SubscriptionAction((ConcreteSource) x)
            ).ToArray();

            Assert.That(subscribedHandlers.Select(x => x.EventName), Is.EquivalentTo(new[]
            {
                nameof(ConcreteSource.TestEvent1),
                nameof(ConcreteSource.TestEvent2),
            }));

            Assert.That(
                subscribedHandlers,
                Has.Exactly(2).Items.With.Property(nameof(SubscribedHandler.EventsHandler)).Not.Null
            );
        }

        [Test]
        public void GetSubscribedHandlers_WithAbstractType_ShouldReturnEventHandlersWithSubscriptions()
        {
            void SubscriptionAction(AbstractSource x)
            {
                x.TestEvent1 += delegate { };
                x.TestEvent1 += delegate { };
                x.TestEvent2 += delegate { };
            }

            var subscribedHandlers = _subscriptionScanService.GetSubscribedHandlers(
                _abstractSourceModel.ClrType,
                _abstractSourceModel.EventFields.Select(x => x.FieldInfo),
                x => SubscriptionAction((AbstractSource)x)
            ).ToArray();

            Assert.That(subscribedHandlers.Select(x => x.EventName), Is.EquivalentTo(new[]
            {
                nameof(AbstractSource.TestEvent1),
                nameof(AbstractSource.TestEvent2),
            }));

            Assert.That(
                subscribedHandlers,
                Has.Exactly(2).Items.With.Property(nameof(SubscribedHandler.EventsHandler)).Not.Null
            );
        }

        public class ConcreteSource
        {
            public event EventHandler TestEvent1;
            public event EventHandler TestEvent2;
            public event EventHandler TestEvent3;

            public ConcreteSource(int p1, int p2)
            {
                
            }
        }

        public abstract class AbstractSource
        {
            public event EventHandler TestEvent1;
            public event EventHandler TestEvent2;
            public event EventHandler TestEvent3;

            protected AbstractSource(int p1, int p2)
            {

            }
        }
    }
}