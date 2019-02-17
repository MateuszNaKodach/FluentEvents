using System;
using FluentEvents.Infrastructure;
using FluentEvents.Pipelines;

namespace FluentEvents.Config
{
    public static class EventConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TSource, TEventArgs> IsForwardedToPipeline<TSource, TEventArgs>(
            this EventConfigurator<TSource, TEventArgs> eventConfigurator
        )
            where TSource : class
            where TEventArgs : class
        {
            var configurator = (IEventConfigurator) eventConfigurator;

            var pipeline = new Pipeline(configurator.EventsContext.Get<IServiceProvider>());

            configurator.SourceModelEventField.AddPipeline(pipeline);

            return new EventPipelineConfigurator<TSource, TEventArgs>(
                pipeline,
                configurator
            );
        }
    }
}
