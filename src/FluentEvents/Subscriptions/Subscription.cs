using System;
using System.Collections.Concurrent;
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
    public class Subscription : ISubscriptionsCancellationToken
    {
        internal Type SourceType { get; }
        private readonly ConcurrentDictionary<string, Delegate> _eventHandlers;

        internal Subscription(Type sourceType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            _eventHandlers = new ConcurrentDictionary<string, Delegate>();
        }

        internal void AddHandler(string eventName, Delegate eventsHandler)
        {
            _eventHandlers.AddOrUpdate(eventName, eventsHandler, (s, d) => Delegate.Combine(eventsHandler, d));
        }

        internal async Task PublishEventAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!SourceType.IsInstanceOfType(pipelineEvent.OriginalSender))
                throw new EventSourceTypeMismatchException();

            if (_eventHandlers.TryGetValue(pipelineEvent.OriginalEventFieldName, out var eventDelegate))
            {
                var exceptions = new List<TargetInvocationException>();
                var invocationList = eventDelegate.GetInvocationList();

                foreach (var eventHandler in invocationList)
                {
                    var isAsync = eventHandler.Method.ReturnType == typeof(Task);
                    try
                    {
                        if (isAsync)
                        {
                            await ((Task) eventHandler.DynamicInvoke(
                                pipelineEvent.OriginalSender,
                                pipelineEvent.OriginalEventArgs
                            )).ConfigureAwait(false);
                        }
                        else
                        {
                            eventHandler.DynamicInvoke(pipelineEvent.OriginalSender, pipelineEvent.OriginalEventArgs);
                        }
                    }
                    catch (TargetInvocationException ex) when (ex.InnerException != null)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Any())
                    throw new SubscriptionPublishAggregateException(exceptions);
            }
        }
    }
}