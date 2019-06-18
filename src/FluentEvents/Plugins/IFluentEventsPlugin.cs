using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Plugins
{
    /// <summary>
    ///     This interface should be implemented by plugins.
    /// </summary>
    public interface IFluentEventsPlugin
    {
        /// <summary>
        ///     Adds the services required for the plugin to the framework's internal <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The framework's internal <see cref="IServiceCollection"/></param>
        void ApplyServices(IServiceCollection services);
    }
}