using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Subscriptions
{
    public class Subscription
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

        internal async Task PublishEvent(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!SourceType.IsInstanceOfType(pipelineEvent.OriginalSender))
                throw new InvalidOperationException("Event source type mismatch");

            if (m_EventHandlers.TryGetValue(pipelineEvent.OriginalEventFieldName, out var eventDelegate))
            {
                var invocationList = eventDelegate.GetInvocationList();

                var asyncEventHandlers = invocationList.Where(x => x.Method.ReturnType == typeof(Task));
                foreach (var asyncEventHandler in asyncEventHandlers)
                    await (Task) asyncEventHandler.DynamicInvoke(pipelineEvent.OriginalSender,
                        pipelineEvent.OriginalEventArgs);

                var syncEventHandlers = invocationList.Where(x => x.Method.ReturnType != typeof(Task));
                foreach (var syncEventHandler in syncEventHandlers)
                    syncEventHandler.DynamicInvoke(pipelineEvent.OriginalSender, pipelineEvent.OriginalEventArgs);
            }
        }
    }
}