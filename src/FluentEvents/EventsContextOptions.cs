using System.Collections.Generic;
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

        void IFluentEventsPluginOptions.AddPlugin(IFluentEventsPlugin plugin) => m_Plugins.Add(plugin);
    }
}