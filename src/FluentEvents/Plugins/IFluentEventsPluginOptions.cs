using System.Collections.Generic;

namespace FluentEvents.Plugins
{
    public interface IFluentEventsPluginOptions
    {
        void AddPlugin(IFluentEventsPlugin plugin);
        IEnumerable<IFluentEventsPlugin> Plugins { get; }
    }
}