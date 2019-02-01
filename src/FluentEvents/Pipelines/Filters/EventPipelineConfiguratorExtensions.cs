using System;
using FluentEvents.Config;

namespace FluentEvents.Pipelines.Filters
{
    public static class EventPipelineConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TSource, TEventArgs> ThenIsFiltered<TSource, TEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TEventArgs, bool> filter)
            where TSource : class 
            where TEventArgs : class 
        {
            ((IEventPipelineConfigurator)eventPipelineConfigurator).Pipeline.AddModule<FilterPipelineModule>(
                new FilterPipelineModuleConfig((sender, args) => filter((TSource)sender, (TEventArgs)args))
            );
            return eventPipelineConfigurator;
        }
    }
}
