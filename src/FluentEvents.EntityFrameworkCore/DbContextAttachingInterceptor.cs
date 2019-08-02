using FluentEvents.Attachment;
using FluentEvents.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FluentEvents.EntityFrameworkCore
{
    internal class DbContextAttachingInterceptor<TDbContext> : IAttachingInterceptor
        where TDbContext : DbContext
    {
        public void OnAttaching(AttachDelegate attach, object source, IEventsScope eventsScope)
        {
            if (source is TDbContext dbContext)
                dbContext.ChangeTracker.Tracked += (sender, args) =>
                {
                    attach(args.Entry.Entity, eventsScope);
                };
        }
    }
}