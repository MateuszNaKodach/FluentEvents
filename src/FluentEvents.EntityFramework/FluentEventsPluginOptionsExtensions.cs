using FluentEvents.Plugins;

namespace FluentEvents.EntityFramework
{
    public static class FluentEventsPluginOptionsExtensions
    {
        public static void AddEntityFramework(this IFluentEventsPluginOptions pluginOptions)
        {
            pluginOptions.AddPlugin(new EntityFrameworkPlugin());
        }
    }
}
