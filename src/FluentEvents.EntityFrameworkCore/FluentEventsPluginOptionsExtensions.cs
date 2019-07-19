using System;
using FluentEvents.Plugins;
using Microsoft.EntityFrameworkCore;

namespace FluentEvents.EntityFrameworkCore
{
    /// <summary>
    ///     Extensions for <see cref="IFluentEventsPluginOptions"/>
    /// </summary>
    public static class FluentEventsPluginOptionsExtensions
    {
        /// <summary>
        ///     Adds a plugin that attaches the entities tracked by a <see cref="DbContext"/>
        ///     to the <see cref="EventsContext"/> automatically.
        /// </summary>
        /// <remarks>
        ///     In order to automatically attach the entities the <see cref="DbContext"/> must be attached
        ///     to the <see cref="EventsContext"/> manually or by using the 
        ///     <see cref="ServiceCollectionExtensions.AddWithEventsAttachedTo{TEventsContext}"/>
        ///     method. (See example)
        /// </remarks>
        /// <example>
        ///     services.AddWithEventsAttachedTo&lt;SampleEventsContext&gt;(() =&gt; {
        ///         services.AddDbContext&lt;MyDbContext&gt;();
        ///     });
        ///         
        ///     services.AddEventsContext&lt;SampleEventsContext&gt;(options =&gt; {
        ///         options.AttachToDbContextEntities&lt;MyDbContext&gt;();
        ///     });
        /// </example>
        /// <param name="pluginOptions">The <see cref="EventsContext"/> options.</param>
        /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The same instance of <see cref="IFluentEventsPluginOptions"/> for chaining.</returns>
        public static IFluentEventsPluginOptions AttachToDbContextEntities<TDbContext>(
            this IFluentEventsPluginOptions pluginOptions
        )
            where TDbContext : DbContext
        {
            if (pluginOptions == null) throw new ArgumentNullException(nameof(pluginOptions));

            pluginOptions.AddPlugin(new EntityFrameworkPlugin<TDbContext>());

            return pluginOptions;
        }
    }
}