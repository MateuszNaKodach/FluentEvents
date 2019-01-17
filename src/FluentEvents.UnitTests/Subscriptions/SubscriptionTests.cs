using System;
using System.Threading.Tasks;
using FluentEvents.Pipelines;
using FluentEvents.Subscriptions;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionTests
    {
        private PipelineEvent m_PipelineEvent;

        private Subscription m_Subscription;

        [SetUp]
        public void SetUp()
        {
            m_PipelineEvent = new PipelineEvent("fieldName", new object(), new object());

            m_Subscription = new Subscription(typeof(object));
        }

        [Test]
        public void AddHandler_ShouldAdd()
        {
            SetUpSubscriptionEventsHandler(m_Subscription, (o, o1) => { });
        }

        [Test]
        public async Task PublishEvent_ShouldPublishToAllHandlers()
        {
            object handlerAction1Sender = null;
            object handlerAction1Args = null;

            void HandlerAction1(object sender, object args)
            {
                handlerAction1Sender = sender;
                handlerAction1Args = args;
            }

            object handlerAction2Sender = null;
            object handlerAction2Args = null;

            void HandlerAction2(object sender, object args)
            {
                handlerAction2Sender = sender;
                handlerAction2Args = args;
            }

            SetUpSubscriptionEventsHandler(m_Subscription, HandlerAction1);
            SetUpSubscriptionEventsHandler(m_Subscription, HandlerAction2);

            await m_Subscription.PublishEventAsync(m_PipelineEvent);

            Assert.That(handlerAction1Sender, Is.EqualTo(m_PipelineEvent.OriginalSender));
            Assert.That(handlerAction2Sender, Is.EqualTo(m_PipelineEvent.OriginalSender));
            Assert.That(handlerAction1Args, Is.EqualTo(m_PipelineEvent.OriginalEventArgs));
            Assert.That(handlerAction2Args, Is.EqualTo(m_PipelineEvent.OriginalEventArgs));
        }

        private void SetUpSubscriptionEventsHandler(Subscription subscription, Action<object, object> handlerAction)
        {
            subscription.AddHandler(m_PipelineEvent.OriginalEventFieldName, handlerAction.GetInvocationList()[0]);
        }
    }
}
