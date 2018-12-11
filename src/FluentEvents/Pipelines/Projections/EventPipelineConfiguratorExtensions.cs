using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using FluentEvents.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Pipelines.Projections
{
    public static class EventPipelineConfiguratorExtensions
    {
        public static EventPipelineConfigurator<TToSource, TToEventArgs> ThenIsProjected<TSource, TToSource, TEventArgs, TToEventArgs>(
            this EventPipelineConfigurator<TSource, TEventArgs> eventPipelineConfigurator,
            Func<TSource, TToSource> senderConverter,
            Func<TEventArgs, TToEventArgs> eventArgsConverter
        )
            where TSource : class
            where TEventArgs : class
            where TToSource : class
            where TToEventArgs : class
        {
            var configurator = (IEventPipelineConfigurator) eventPipelineConfigurator;
            var serviceProvider = configurator.EventsContext.Get<IServiceProvider>();
            var sourceModelsService = serviceProvider.GetRequiredService<ISourceModelsService>();

            var projectedSourceModel = sourceModelsService.GetOrCreateSourceModel(
                typeof(TToSource),
                configurator.SourceModel.EventsContext
            );

            var projectedEventField = projectedSourceModel.GetOrCreateEventField(
                configurator.SourceModelEventField.Name
            );

            var projectionPipelineModuleConfig = new ProjectionPipelineModuleConfig(
                new EventsSenderProjection<TSource, TToSource>(senderConverter),
                new EventArgsProjection<TEventArgs, TToEventArgs>(eventArgsConverter)
            );

            configurator.Pipeline.AddModuleConfig(projectionPipelineModuleConfig);

            return new EventPipelineConfigurator<TToSource, TToEventArgs>(
                projectedSourceModel,
                projectedEventField,
                configurator
            );
        }
    }
}
