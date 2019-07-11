using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class Subscription
    {
        internal Type EventType { get; }
        private readonly Delegate _eventsHandler;

        internal Subscription(Type sourceType, Delegate eventsHandler)
        {
            EventType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            _eventsHandler = eventsHandler;
        }

        internal async Task PublishEventAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!EventType.IsInstanceOfType(pipelineEvent.Event))
                throw new EventTypeMismatchException();

            var exceptions = new List<TargetInvocationException>();
            var invocationList = _eventsHandler.GetInvocationList();

            foreach (var eventHandler in invocationList)
            {
                try
                {
                    await InvokeEventHandlerAsync(pipelineEvent, eventHandler).ConfigureAwait(false);
                }
                catch (TargetInvocationException ex) when (ex.InnerException != null)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new SubscriptionPublishAggregateException(exceptions);
        }

        private static Task InvokeEventHandlerAsync(PipelineEvent pipelineEvent, Delegate eventHandler)
        {
            var isAsync = eventHandler.Method.ReturnType == typeof(Task);

            if (isAsync)
                return (Task) eventHandler.DynamicInvoke(pipelineEvent.Event);

            eventHandler.DynamicInvoke(pipelineEvent.Event);

            return Task.CompletedTask;
        }
    }
}