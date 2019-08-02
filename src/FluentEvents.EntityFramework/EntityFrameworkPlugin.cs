using System.Data.Entity;
using FluentEvents.Attachment;
using FluentEvents.Plugins;
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
