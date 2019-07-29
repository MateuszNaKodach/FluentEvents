using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentEvents.Pipelines
{
    internal class Pipeline : IPipeline
    {
        private readonly IServiceProvider _internalServiceProvider;
        private readonly ICollection<ModuleProxy> _moduleProxies;
        private NextModuleDelegate _nextModule;

        public Pipeline(IServiceProvider internalServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider;
            _moduleProxies = new List<ModuleProxy>();
            _nextModule = null;
        }

        public void AddModule<TModule, TConfig>(TConfig moduleConfig) where TModule : IPipelineModule<TConfig>
        {
            if (moduleConfig == null)
                throw new ArgumentNullException(nameof(moduleConfig));

            var module = _internalServiceProvider.GetService(typeof(TModule));
            if (module == null)
                throw new PipelineModuleNotFoundException();

            _moduleProxies.Add(new ModuleProxy<TConfig>(typeof(TModule), moduleConfig));
        }

        public Task ProcessEventAsync(
            PipelineEvent pipelineEvent,
            IEventsScope eventsScope
        )
        {
            var pipeline = _nextModule ?? (_nextModule = Build());
            return pipeline(new PipelineContext(pipelineEvent, eventsScope));
        }

        private NextModuleDelegate Build()
        {
            var stack = new Stack<ModuleProxy>(_moduleProxies.Reverse());
            var next = GetNextModuleDelegate(stack);
            return next;
        }

        private NextModuleDelegate GetNextModuleDelegate(Stack<ModuleProxy> moduleProxies)
        {
            var moduleProxy = moduleProxies.Count > 0 ? moduleProxies.Pop() : null;
            if (moduleProxy == null)
                return x => Task.CompletedTask;

            var next = GetNextModuleDelegate(moduleProxies);
            var moduleType = moduleProxy.Type;
            var loggerType = typeof(ILogger<>).MakeGenericType(moduleType);

            Task NextModuleDelegate(PipelineContext pipelineContext)
            {
                var logger = (ILogger) _internalServiceProvider.GetRequiredService(loggerType);
                logger.InvokingPipelineModule(moduleType, pipelineContext.PipelineEvent);

                return moduleProxy.InvokeAsync(_internalServiceProvider, pipelineContext, next);
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

            public abstract Task InvokeAsync(
                IServiceProvider serviceProvider,
                PipelineContext pipelineContext,
                NextModuleDelegate next
            );
        }

        private class ModuleProxy<TConfig> : ModuleProxy
        {
            private readonly TConfig _config;

            public ModuleProxy(Type type, TConfig config) : base(type)
            {
                _config = config;
            }

            public override Task InvokeAsync(
                IServiceProvider serviceProvider, 
                PipelineContext pipelineContext,
                NextModuleDelegate next
            )
            {
                var module = (IPipelineModule<TConfig>) serviceProvider.GetRequiredService(Type);
           
                return module.InvokeAsync(_config, pipelineContext, next);
            }
        }
    }
}