using System.Collections.Generic;

namespace FluentEvents.Plugins
{
    /// <summary>
    ///     Provides a way to add or list plugins.
    /// </summary>
    public interface IFluentEventsPluginOptions
    {
        /// <summary>
        ///     Adds a plugin.
        /// </summary>
        /// <remarks>
        ///     This method should be called with custom extension methods
        ///     to provide a better configuration experience.
        /// </remarks>
        /// <param name="plugin">The instance of the plugin.</param>
        void AddPlugin(IFluentEventsPlugin plugin);

        /// <summary>
        ///     Returns the list of the added plugins.
        /// </summary>
        IEnumerable<IFluentEventsPlugin> Plugins { get; }
    }
}