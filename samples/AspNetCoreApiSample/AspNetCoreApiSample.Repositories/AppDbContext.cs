using System.Threading;
using System.Threading.Tasks;
using AspNetCoreApiSample.Domain;
using AspNetCoreApiSample.Events;
using FluentEvents;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreApiSample.Repositories
{
    public class AppDbContext : DbContext
    {
        private readonly AppEventsContext _appEventsContext;
        private readonly EventsScope _eventsScope;

        public AppDbContext(
            AppEventsContext appEventsContext,
            EventsScope eventsScope, 
            DbContextOptions<AppDbContext> options
        ) : base(options)
        {
            _appEventsContext = appEventsContext;
            _eventsScope = eventsScope;
        }

        public DbSet<Contract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contract>();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await _appEventsContext.ProcessQueuedEventsAsync(_eventsScope, AppEventsContext.AfterSaveChangesQueueName);

            return result;
        }
    }
}
