using System.Data.Entity;
using FluentEvents.Plugins;
using FluentEvents.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.EntityFramework
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
