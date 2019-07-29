using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    internal class Subscription
    {
        internal Type EventType { get; }
        private readonly Delegate _eventsHandler;

        internal Subscription(Type sourceType, Delegate eventsHandler)
        {
            EventType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            _eventsHandler = eventsHandler ?? throw new ArgumentNullException(nameof(eventsHandler));
        }

        internal async Task InvokeEventsHandlerAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!EventType.IsInstanceOfType(pipelineEvent.Event))
                throw new EventTypeMismatchException();

            try
            {
                await InvokeEventsHandlerAsync(pipelineEvent, _eventsHandler).ConfigureAwait(false);
            }
            catch (TargetInvocationException ex)
            {
                throw new SubscribedEventHandlerThrewException(ex);
            }
        }

        private static Task InvokeEventsHandlerAsync(PipelineEvent pipelineEvent, Delegate eventHandler)
        {
            var isAsync = eventHandler.Method.ReturnType == typeof(Task);

            if (isAsync)
                return (Task) eventHandler.DynamicInvoke(pipelineEvent.Event);

            eventHandler.DynamicInvoke(pipelineEvent.Event);

            return Task.CompletedTask;
        }
    }
}