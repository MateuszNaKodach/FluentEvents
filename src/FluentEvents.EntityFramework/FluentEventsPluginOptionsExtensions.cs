using System.Data.Entity;
using FluentEvents.Plugins;

namespace FluentEvents.EntityFramework
{
    public static class FluentEventsPluginOptionsExtensions
    {
        public static IFluentEventsPluginOptions AttachToDbContextEntities<TDbContext>(
            this IFluentEventsPluginOptions pluginOptions
        )
            where TDbContext : DbContext
        {
            pluginOptions.AddPlugin(new EntityFrameworkPlugin<TDbContext>());
            return pluginOptions;
        }
    }
}