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
        public string QueueName { get; }

        private readonly IServiceProvider m_InternalServiceProvider;
        private readonly ICollection<ModuleTypeConfigPair> m_ModuleConfigs;
        private NextModuleDelegate m_NextModule;

        internal Pipeline(string queueName, IServiceProvider internalServiceProvider)
        {
            QueueName = queueName;
            m_InternalServiceProvider = internalServiceProvider;
            m_ModuleConfigs = new List<ModuleTypeConfigPair>();
            m_NextModule = null;
        }

        public void AddModule<TModule>(object moduleConfig) where TModule : IPipelineModule
        {
            if (moduleConfig == null)
                throw new ArgumentNullException(nameof(moduleConfig));

            m_ModuleConfigs.Add(new ModuleTypeConfigPair(typeof(TModule), moduleConfig));
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
            var stack = new Stack<ModuleTypeConfigPair>(m_ModuleConfigs.Reverse());
            var next = GetNextModuleDelegate(stack);
            return next;
        }

        private static NextModuleDelegate GetNextModuleDelegate(Stack<ModuleTypeConfigPair> moduleConfigs)
        {
            var moduleTypeConfigPair = moduleConfigs.Count > 0 ? moduleConfigs.Pop() : null;
            if (moduleTypeConfigPair == null)
                return x => Task.CompletedTask;

            var next = GetNextModuleDelegate(moduleConfigs);
            var moduleType = moduleTypeConfigPair.Type;
            var loggerType = typeof(ILogger<>).MakeGenericType(moduleType);

            async Task NextModuleDelegate(PipelineContext pipelineContext)
            {
                var module = (IPipelineModule) pipelineContext.ServiceProvider.GetService(moduleType);
                if (module == null) throw new PipelineModuleNotFoundException();

                var logger = (ILogger) pipelineContext.ServiceProvider.GetRequiredService(loggerType);

                logger.InvokingPipelineModule(moduleType, pipelineContext.PipelineEvent);

                var pipelineModuleContext = new PipelineModuleContext(moduleTypeConfigPair.Config, pipelineContext);

                await module.InvokeAsync(pipelineModuleContext, next);
            }

            return NextModuleDelegate;
        }

        private class ModuleTypeConfigPair
        {
            public Type Type { get; }
            public object Config { get; }

            public ModuleTypeConfigPair(Type type, object config)
            {
                Type = type;
                Config = config;
            }
        }
    }
}