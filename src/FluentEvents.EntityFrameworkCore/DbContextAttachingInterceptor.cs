using FluentEvents.Routing;
using Microsoft.EntityFrameworkCore;

namespace FluentEvents.EntityFrameworkCore
{
    internal class DbContextAttachingInterceptor<TDbContext> : IAttachingInterceptor
        where TDbContext : DbContext
    {
        private readonly IAttachingService m_AttachingService;

        public DbContextAttachingInterceptor(IAttachingService attachingService)
        {
            m_AttachingService = attachingService;
        }

        public void OnAttaching(object source, EventsScope eventsScope)
        {
            if (source is TDbContext dbContext)
                dbContext.ChangeTracker.Tracked += (sender, args) =>
                {
                    m_AttachingService.Attach(args.Entry.Entity, eventsScope);
                };
        }
    }
}