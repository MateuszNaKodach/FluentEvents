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
        private SubscriptionScanService m_SubscriptionScanService;
        private SourceModel m_ConcreteSourceModel;
        private SourceModel m_AbstractSourceModel;

        [SetUp]
        public void SetUp()
        {
            m_ConcreteSourceModel = new SourceModel(typeof(ConcreteSource));
            m_ConcreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent1));
            m_ConcreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent2));
            m_ConcreteSourceModel.GetOrCreateEventField(nameof(ConcreteSource.TestEvent3));

            m_AbstractSourceModel = new SourceModel(typeof(AbstractSource));
            m_AbstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent1));
            m_AbstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent2));
            m_AbstractSourceModel.GetOrCreateEventField(nameof(AbstractSource.TestEvent3));

            m_SubscriptionScanService = new SubscriptionScanService();
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

            var subscribedHandlers = m_SubscriptionScanService.GetSubscribedHandlers(
                m_ConcreteSourceModel,
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

            var subscribedHandlers = m_SubscriptionScanService.GetSubscribedHandlers(
                m_AbstractSourceModel,
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