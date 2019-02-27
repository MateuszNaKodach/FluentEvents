using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FluentEvents.Routing;

namespace FluentEvents.EntityFramework
{
    internal class DbContextAttachingInterceptor<TDbContext> : IAttachingInterceptor
        where TDbContext : DbContext
    {
        public void OnAttaching(IAttachingService attachingService, object source, EventsScope eventsScope)
        {
            if (source is TDbContext dbContext)
                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    attachingService.Attach(args.Entity, eventsScope);
                };
        }
    }
}