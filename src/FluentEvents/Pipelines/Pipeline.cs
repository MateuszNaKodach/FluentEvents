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
        private readonly IServiceProvider _internalServiceProvider;
        private readonly ICollection<ModuleProxy> _moduleProxies;
        private NextModuleDelegate _nextModule;

        internal Pipeline(IServiceProvider internalServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider;
            _moduleProxies = new List<ModuleProxy>();
            _nextModule = null;
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

            var module = _internalServiceProvider.GetService(typeof(TModule));
            if (module == null)
                throw new PipelineModuleNotFoundException();

            _moduleProxies.Add(new ModuleProxy<TConfig>(typeof(TModule), moduleConfig));
        }

        /// <inheritdoc />
        public async Task ProcessEventAsync(
            PipelineEvent pipelineEvent,
            EventsScope eventsScope
        )
        {
            var pipeline = _nextModule ?? (_nextModule = Build());
            await pipeline(new PipelineContext(pipelineEvent, eventsScope, _internalServiceProvider));
        }

        private NextModuleDelegate Build()
        {
            var stack = new Stack<ModuleProxy>(_moduleProxies.Reverse());
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
            private readonly TConfig _config;

            public ModuleProxy(Type type, TConfig config) : base(type)
            {
                _config = config;
            }

            public override Task InvokeAsync(PipelineContext pipelineContext, NextModuleDelegate next)
            {
                var module = (IPipelineModule<TConfig>) pipelineContext.ServiceProvider.GetRequiredService(Type);
           
                return module.InvokeAsync(_config, pipelineContext, next);
            }
        }
    }
}