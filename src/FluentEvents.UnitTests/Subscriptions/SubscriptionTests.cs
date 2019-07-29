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
        private readonly Exception _exception = new Exception();

        private PipelineEvent _pipelineEvent;

        private Subscription _subscription;
        private Subscription _asyncSubscription;

        private bool _isThrowingEnabled;
        private object _handlerActionEvent;


        [SetUp]
        public void SetUp()
        {
            _pipelineEvent = new PipelineEvent(new TestEvent());

            _subscription = new Subscription(typeof(TestEvent), new Action<object>(HandlerAction));
            _asyncSubscription = new Subscription(typeof(TestEvent), new Func<object, Task>(HandlerActionAsync));

            _isThrowingEnabled = false;
            _handlerActionEvent = null;
        }
        
        [Test]
        public void InvokeEventsHandlerAsync_WithNullPipelineEvent_ShouldThrow()
        {
            Assert.That(async () => {
                await _subscription.InvokeEventsHandlerAsync(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeEventsHandlerAsync_WithEventTypeMismatch_ShouldThrow()
        {
            var pipelineEvent = new PipelineEvent(123);

            Assert.That(async () =>
            {
                await _subscription.InvokeEventsHandlerAsync(pipelineEvent);
            }, Throws.TypeOf<EventTypeMismatchException>());
        }

        [Test]
        public async Task InvokeEventsHandlerAsync_ShouldInvokeHandler([Values] bool isAsync)
        {
            var subscription = isAsync ? _asyncSubscription : _subscription;

            await subscription.InvokeEventsHandlerAsync(_pipelineEvent);

            Assert.That(_handlerActionEvent, Is.EqualTo(_pipelineEvent.Event));
        }

        [Test]
        public void InvokeEventsHandlerAsync_ShouldCatchAndReturnTargetInvocationExceptions([Values] bool isAsync)
        {
            var subscription = isAsync ? _asyncSubscription : _subscription;

            _isThrowingEnabled = true;

            Assert.That(
                async () => { await subscription.InvokeEventsHandlerAsync(_pipelineEvent); },
                Throws.TypeOf<SubscribedEventHandlerThrewException>()
                    .And
                    .With.Property(nameof(Exception.InnerException)).EqualTo(_exception)
            );
        }
        
        private void HandlerAction(object args)
        {
            ThrowIfEnabled();
            _handlerActionEvent = args;
        }

        private Task HandlerActionAsync(object args)
        {
            ThrowIfEnabled();
            _handlerActionEvent = args;
            return Task.CompletedTask;
        }

        private void ThrowIfEnabled()
        {
            if (_isThrowingEnabled)
                throw _exception;
        }

        private class TestEvent { }
    }
}
