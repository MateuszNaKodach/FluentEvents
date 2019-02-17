using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    public class Subscription : ISubscriptionsCancellationToken
    {
        internal Type SourceType { get; }
        private readonly ConcurrentDictionary<string, Delegate> m_EventHandlers;

        internal Subscription(Type sourceType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            m_EventHandlers = new ConcurrentDictionary<string, Delegate>();
        }

        internal void AddHandler(string eventName, Delegate eventsHandler)
        {
            m_EventHandlers.AddOrUpdate(eventName, eventsHandler, (s, d) => Delegate.Combine(eventsHandler, d));
        }

        internal async Task PublishEventAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!SourceType.IsInstanceOfType(pipelineEvent.OriginalSender))
                throw new EventSourceTypeMismatchException();

            if (m_EventHandlers.TryGetValue(pipelineEvent.OriginalEventFieldName, out var eventDelegate))
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
                            await (Task) eventHandler.DynamicInvoke(
                                pipelineEvent.OriginalSender,
                                pipelineEvent.OriginalEventArgs
                            );
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