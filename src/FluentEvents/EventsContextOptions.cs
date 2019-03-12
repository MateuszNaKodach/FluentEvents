using System;
using System.Collections.Generic;
using System.Linq;
using FluentEvents.Plugins;

namespace FluentEvents
{
    /// <summary>
    ///     The options used by an <see cref="EventsContext"/>
    /// </summary>
    public class EventsContextOptions : IFluentEventsPluginOptions
    {
        private readonly IList<IFluentEventsPlugin> m_Plugins;
        IEnumerable<IFluentEventsPlugin> IFluentEventsPluginOptions.Plugins => m_Plugins;

        /// <summary>
        ///     Initializes a new instance.
        /// </summary>
        public EventsContextOptions()
        {
            m_Plugins = new List<IFluentEventsPlugin>();
        }

        void IFluentEventsPluginOptions.AddPlugin(IFluentEventsPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            if (m_Plugins.Any(x => x.GetType() == plugin.GetType()))
                throw new DuplicatePluginException();

            m_Plugins.Add(plugin);
        }
    }
}