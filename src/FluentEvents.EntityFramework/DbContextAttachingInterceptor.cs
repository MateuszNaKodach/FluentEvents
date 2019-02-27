using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FluentEvents.Routing;

namespace FluentEvents.EntityFramework
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
                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    m_AttachingService.Attach(args.Entity, eventsScope);
                };
        }
    }
}