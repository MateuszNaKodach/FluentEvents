using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FluentEvents.Attachment;
using FluentEvents.Infrastructure;

namespace FluentEvents.EntityFramework
{
    internal class DbContextAttachingInterceptor<TDbContext> : IAttachingInterceptor
        where TDbContext : DbContext
    {
        public void OnAttaching(AttachDelegate attach, object source, IEventsScope eventsScope)
        {
            if (source is TDbContext dbContext)
                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectMaterialized += (sender, args) =>
                {
                    attach(args.Entity, eventsScope);
                };
        }
    }
}