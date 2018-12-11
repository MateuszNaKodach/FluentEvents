using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Pipelines
{
    public class Pipeline : IPipeline
    {
        public IInfrastructureEventsContext EventsContext { get; }
        public string QueueName { get; }

        private readonly IServiceProvider m_InternalServiceProvider;
        private readonly ICollection<IPipelineModuleConfig> m_ModuleConfigs;
        private NextModuleDelegate m_NextModule;

        internal Pipeline(string queueName, IInfrastructureEventsContext eventsContext, IServiceProvider internalServiceProvider)
        {
            QueueName = queueName;
            EventsContext = eventsContext;
            m_InternalServiceProvider = internalServiceProvider;
            m_ModuleConfigs = new List<IPipelineModuleConfig>();
            m_NextModule = null;
        }

        public void AddModuleConfig(IPipelineModuleConfig moduleConfig)
        {
            if (moduleConfig == null)
                throw new ArgumentNullException(nameof(moduleConfig));

            m_ModuleConfigs.Add(moduleConfig);
        }

        public async Task ProcessEventAsync(
            PipelineEvent pipelineEvent,
            EventsScope eventsScope
        )
        {
            using (var serviceScope = m_InternalServiceProvider.CreateScope())
            {
                var pipeline = m_NextModule ?? (m_NextModule = Build());
                await pipeline(new PipelineContext(pipelineEvent, eventsScope, serviceScope.ServiceProvider));
            }
        }

        private NextModuleDelegate Build()
        {
            var stack = new Stack<IPipelineModuleConfig>(m_ModuleConfigs.Reverse());
            var next = GetNextModuleDelegate(stack);
            return next;
        }

        private static NextModuleDelegate GetNextModuleDelegate(Stack<IPipelineModuleConfig> moduleConfigs)
        {
            var moduleConfig = moduleConfigs.Count > 0 ? moduleConfigs.Pop() : null;
            if (moduleConfig == null)
                return x => Task.CompletedTask;

            var next = GetNextModuleDelegate(moduleConfigs);
            var moduleType = moduleConfig.ModuleType;
            var loggerType = typeof(ILogger<>).MakeGenericType(moduleType);

            async Task NextModuleDelegate(PipelineContext pipelineContext)
            {
                var module = (IPipelineModule) pipelineContext.ServiceProvider.GetService(moduleType);
                if (module == null) throw new PipelineModuleNotFoundException();

                var logger = (ILogger) pipelineContext.ServiceProvider.GetRequiredService(loggerType);

                logger.InvokingPipelineModule(moduleType, pipelineContext.PipelineEvent);

                var pipelineModuleContext = new PipelineModuleContext(moduleConfig, pipelineContext);

                await module.InvokeAsync(pipelineModuleContext, next);
            }

            return NextModuleDelegate;
        }
    }
}