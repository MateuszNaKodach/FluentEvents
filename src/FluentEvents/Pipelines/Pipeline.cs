using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Pipelines
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class Pipeline : IPipeline
    {
        private readonly IServiceProvider m_InternalServiceProvider;
        private readonly ICollection<ModuleProxy> m_ModuleProxies;
        private NextModuleDelegate m_NextModule;

        internal Pipeline(IServiceProvider internalServiceProvider)
        {
            m_InternalServiceProvider = internalServiceProvider;
            m_ModuleProxies = new List<ModuleProxy>();
            m_NextModule = null;
        }

        /// <summary>
        ///     Adds a module with the associated configuration to the pipeline.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <typeparam name="TConfig">The type of the module configuration.</typeparam>
        /// <param name="moduleConfig">An instance of the module configuration</param>
        public void AddModule<TModule, TConfig>(TConfig moduleConfig) where TModule : IPipelineModule<TConfig>
        {
            if (moduleConfig == null)
                throw new ArgumentNullException(nameof(moduleConfig));

            m_ModuleProxies.Add(new ModuleProxy<TConfig>(typeof(TModule), moduleConfig));
        }

        /// <inheritdoc />
        public async Task ProcessEventAsync(
            PipelineEvent pipelineEvent,
            EventsScope eventsScope
        )
        {
            var pipeline = m_NextModule ?? (m_NextModule = Build());
            await pipeline(new PipelineContext(pipelineEvent, eventsScope, m_InternalServiceProvider));
        }

        private NextModuleDelegate Build()
        {
            var stack = new Stack<ModuleProxy>(m_ModuleProxies.Reverse());
            var next = GetNextModuleDelegate(stack);
            return next;
        }

        private static NextModuleDelegate GetNextModuleDelegate(Stack<ModuleProxy> moduleProxies)
        {
            var moduleProxy = moduleProxies.Count > 0 ? moduleProxies.Pop() : null;
            if (moduleProxy == null)
                return x => Task.CompletedTask;

            var next = GetNextModuleDelegate(moduleProxies);
            var moduleType = moduleProxy.Type;
            var loggerType = typeof(ILogger<>).MakeGenericType(moduleType);

            Task NextModuleDelegate(PipelineContext pipelineContext)
            {
                var logger = (ILogger) pipelineContext.ServiceProvider.GetRequiredService(loggerType);
                logger.InvokingPipelineModule(moduleType, pipelineContext.PipelineEvent);

                return moduleProxy.InvokeAsync(pipelineContext, next);
            }

            return NextModuleDelegate;
        }

        private abstract class ModuleProxy
        {
            public Type Type { get; }

            protected ModuleProxy(Type type)
            {
                Type = type;
            }

            public abstract Task InvokeAsync(PipelineContext pipelineContext, NextModuleDelegate next);
        }

        private class ModuleProxy<TConfig> : ModuleProxy
        {
            private readonly TConfig m_Config;

            public ModuleProxy(Type type, TConfig config) : base(type)
            {
                m_Config = config;
            }

            public override Task InvokeAsync(PipelineContext pipelineContext, NextModuleDelegate next)
            {
                var module = (IPipelineModule<TConfig>) pipelineContext.ServiceProvider.GetService(Type);
                if (module == null)
                    throw new PipelineModuleNotFoundException();

                return module.InvokeAsync(m_Config, pipelineContext, next);
            }
        }
    }
}