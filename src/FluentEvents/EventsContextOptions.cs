using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Plugins;

namespace FluentEvents
{
    /// <summary>
    ///     The options used by an <see cref="EventsContext"/>
    /// </summary>
    public sealed class EventsContextOptions : IFluentEventsPluginOptions
    {
        private readonly IList<IFluentEventsPlugin> _plugins;
        IEnumerable<IFluentEventsPlugin> IFluentEventsPluginOptions.Plugins => _plugins;

        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public EventsContextOptions()
        {
            _plugins = new List<IFluentEventsPlugin>();
        }

        void IFluentEventsPluginOptions.AddPlugin(IFluentEventsPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            if (_plugins.Any(x => x.GetType() == plugin.GetType()))
                throw new DuplicatePluginException();

            _plugins.Add(plugin);
        }
    }
}