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
        private PipelineEvent _pipelineEvent;

        private Subscription _subscription;

        [SetUp]
        public void SetUp()
        {
            _pipelineEvent = new PipelineEvent(
                typeof(object),
                "fieldName",
                new object(),
                new object()
            );

            _subscription = new Subscription(typeof(object));
        }

        [Test]
        public void AddHandler_ShouldAdd()
        {
            SetUpSubscriptionEventsHandler(_subscription, (o, o1) => { });
        }

        [Test]
        public void PublishEventAsync_WithNullPipelineEvent_ShouldThrow()
        {
            Assert.That(async () => {
                await _subscription.PublishEventAsync(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PublishEventAsync_WithEventSourceTypeMismatch_ShouldThrow()
        {
            var pipelineEvent = new PipelineEvent(typeof(int), "", null, null);

            Assert.That(async () =>
            {
                await _subscription.PublishEventAsync(pipelineEvent);
            }, Throws.TypeOf<EventSourceTypeMismatchException>());
        }

        [Test]
        public async Task PublishEvent_ShouldPublishToAllHandlers([Values] bool isAsync)
        {
            object handlerAction1Sender = null;
            object handlerAction1Args = null;

            void HandlerAction1(object sender, object args)
            {
                handlerAction1Sender = sender;
                handlerAction1Args = args;
            }

            Task HandlerAction1Async(object sender, object args)
            {
                handlerAction1Sender = sender;
                handlerAction1Args = args;
                return Task.CompletedTask;
            }

            object handlerAction2Sender = null;
            object handlerAction2Args = null;

            void HandlerAction2(object sender, object args)
            {
                handlerAction2Sender = sender;
                handlerAction2Args = args;
            }

            Task HandlerAction2Async(object sender, object args)
            {
                handlerAction2Sender = sender;
                handlerAction2Args = args;
                return Task.CompletedTask;
            }

            if (isAsync)
            {
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction1Async);
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction2Async);
            }
            else
            {
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction1);
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction2);
            }

            await _subscription.PublishEventAsync(_pipelineEvent);

            Assert.That(handlerAction1Sender, Is.EqualTo(_pipelineEvent.OriginalSender));
            Assert.That(handlerAction2Sender, Is.EqualTo(_pipelineEvent.OriginalSender));
            Assert.That(handlerAction1Args, Is.EqualTo(_pipelineEvent.OriginalEventArgs));
            Assert.That(handlerAction2Args, Is.EqualTo(_pipelineEvent.OriginalEventArgs));
        }

        [Test]
        public void PublishEvent_ShouldCatchAndRethrowAggregatedHandlerExceptions([Values] bool isAsync)
        {
            void HandlerAction1(object sender, object args) => throw new Exception();
            Task HandlerAction1Async(object sender, object args) => throw new Exception();
            void HandlerAction2(object sender, object args) => throw new Exception();
            Task HandlerAction2Async(object sender, object args) => throw new Exception();

            if (isAsync)
            {
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction1Async);
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction2Async);
            }
            else
            {
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction1);
                SetUpSubscriptionEventsHandler(_subscription, HandlerAction2);
            }

            Assert.That(async () =>
            {
                await _subscription.PublishEventAsync(_pipelineEvent);
            }, Throws.TypeOf<SubscriptionPublishAggregateException>()
                .With
                .Property(nameof(AggregateException.InnerExceptions)).Count.EqualTo(2));
        }

        private void SetUpSubscriptionEventsHandler(Subscription subscription, Action<object, object> handlerAction)
        {
            subscription.AddHandler(_pipelineEvent.OriginalEventFieldName, handlerAction.GetInvocationList()[0]);
        }

        private void SetUpSubscriptionEventsHandler(Subscription subscription, Func<object, object, Task> handlerAction)
        {
            subscription.AddHandler(_pipelineEvent.OriginalEventFieldName, handlerAction.GetInvocationList()[0]);
        }
    }
}
