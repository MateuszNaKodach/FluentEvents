using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
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

        internal async Task PublishEventAsync(PipelineEvent pipelineEvent)
        {
            if (pipelineEvent == null) throw new ArgumentNullException(nameof(pipelineEvent));
            if (!SourceType.IsInstanceOfType(pipelineEvent.OriginalSender))
                throw new InvalidOperationException("Event source type mismatch");

            if (m_EventHandlers.TryGetValue(pipelineEvent.OriginalEventFieldName, out var eventDelegate))
            {
                var invocationList = eventDelegate.GetInvocationList();

                var asyncEventHandlers = invocationList.Where(x => x.Method.ReturnType == typeof(Task));
                foreach (var asyncEventHandler in asyncEventHandlers)
                {
                    await PublishAndHandleExceptionAsync(true, pipelineEvent, asyncEventHandler);
                }

                var syncEventHandlers = invocationList.Where(x => x.Method.ReturnType != typeof(Task));
                foreach (var syncEventHandler in syncEventHandlers)
                {
                    try
                    {
                        await PublishAndHandleExceptionAsync(false, pipelineEvent, syncEventHandler);
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw new SubscriptionPublishException(ex);
                    }
                }
            }
        }

        private static async Task PublishAndHandleExceptionAsync(bool isAsync, PipelineEvent pipelineEvent, Delegate eventHandler)
        {
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
                throw new SubscriptionPublishException(ex);
            }
        }
    }
}