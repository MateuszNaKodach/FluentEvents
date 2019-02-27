using FluentEvents.Plugins;
using FluentEvents.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.EntityFrameworkCore
{
    internal class EntityFrameworkPlugin<TDbContext> : IFluentEventsPlugin
        where TDbContext : DbContext
    {
        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton<IAttachingInterceptor, DbContextAttachingInterceptor<TDbContext>>();
        }
    }
}
