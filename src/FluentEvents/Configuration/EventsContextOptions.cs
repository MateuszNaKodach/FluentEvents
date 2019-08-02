using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Plugins;

namespace FluentEvents.Configuration
{
    /// <summary>
    ///     The options used by an <see cref="EventsContext"/>
    /// </summary>
    public sealed class EventsContextOptions : IEventsContextOptions
    {
        private readonly IList<IFluentEventsPlugin> _plugins;
        IEnumerable<IFluentEventsPlugin> IEventsContextOptions.Plugins => _plugins;

        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public EventsContextOptions()
        {
            _plugins = new List<IFluentEventsPlugin>();
        }

        void IEventsContextOptions.AddPlugin(IFluentEventsPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            if (_plugins.Any(x => x.GetType() == plugin.GetType()))
                throw new DuplicatePluginException();

            _plugins.Add(plugin);
        }
    }
}