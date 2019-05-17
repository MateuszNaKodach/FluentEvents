using System.Threading;
using System.Threading.Tasks;
using FluentEvents;
using Microsoft.EntityFrameworkCore;
using WorkerSample.DomainModel;
using WorkerSample.Events;

namespace WorkerSample.Repositories
{
    internal class AppDbContext : DbContext
    {
        private readonly AppEventsContext _appEventsContext;
        private readonly EventsScope _eventsScope;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            AppEventsContext appEventsContext,
            EventsScope eventsScope
        ) 
            : base(options)
        {
            _appEventsContext = appEventsContext;
            _eventsScope = eventsScope;
        }

        public DbSet<ProductSubscription> ProductSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductSubscription>();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await _appEventsContext.ProcessQueuedEventsAsync(_eventsScope, AppEventsContext.AfterSaveChangesQueueName);

            return result;
        }
    }
}
