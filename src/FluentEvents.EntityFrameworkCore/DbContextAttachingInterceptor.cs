using FluentEvents.Routing;
using Microsoft.EntityFrameworkCore;

namespace FluentEvents.EntityFrameworkCore
{
    internal class DbContextAttachingInterceptor<TDbContext> : IAttachingInterceptor
        where TDbContext : DbContext
    {
        public void OnAttaching(IAttachingService attachingService, object source, EventsScope eventsScope)
        {
            if (source is TDbContext dbContext)
                dbContext.ChangeTracker.Tracked += (sender, args) =>
                {
                    attachingService.Attach(args.Entry.Entity, eventsScope);
                };
        }
    }
}