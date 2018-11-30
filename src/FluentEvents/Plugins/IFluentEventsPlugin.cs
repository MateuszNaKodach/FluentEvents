using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Plugins
{
    public interface IFluentEventsPlugin
    {
        void ApplyServices(IServiceCollection services);
    }
}