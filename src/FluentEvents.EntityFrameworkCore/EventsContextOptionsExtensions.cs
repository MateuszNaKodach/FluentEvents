using System;
using FluentEvents.Configuration;
using FluentEvents.Plugins;
using Microsoft.EntityFrameworkCore;

namespace FluentEvents.EntityFrameworkCore
{
    /// <summary>
    ///     Extensions for <see cref="IEventsContextOptions"/>
    /// </summary>
    public static class EventsContextOptionsExtensions
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
        /// <param name="options">The <see cref="EventsContext"/> options.</param>
        /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
        /// <returns>The same instance of <see cref="IEventsContextOptions"/> for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IEventsContextOptions AttachToDbContextEntities<TDbContext>(
            this IEventsContextOptions options
        )
            where TDbContext : DbContext
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AddPlugin(new EntityFrameworkPlugin<TDbContext>());

            return options;
        }
    }
}