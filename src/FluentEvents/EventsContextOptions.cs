using System.Collections.Generic;
using FluentEvents.Plugins;

namespace FluentEvents
{
    public class EventsContextOptions : IFluentEventsPluginOptions
    {
        private readonly IList<IFluentEventsPlugin> m_Plugins;
        IEnumerable<IFluentEventsPlugin> IFluentEventsPluginOptions.Plugins => m_Plugins;

        public EventsContextOptions()
        {
            m_Plugins = new List<IFluentEventsPlugin>();
        }

        void IFluentEventsPluginOptions.AddPlugin(IFluentEventsPlugin plugin) => m_Plugins.Add(plugin);
    }
}