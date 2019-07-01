using System.Threading;
using System.Threading.Tasks;
using AzureSignalRSample.Domain;
using AzureSignalRSample.Events;
using FluentEvents;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Persistence
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

        public DbSet<LightBulb> LightBulbs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LightBulb>().Property<int>("Id");
            modelBuilder.Entity<LightBulb>().HasKey("Id");
            modelBuilder.Entity<LightBulb>().HasData(new {Id = 1, IsOn = false});
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await _appEventsContext.ProcessQueuedEventsAsync(_eventsScope, AppEventsContext.AfterSaveChangesQueueName);

            return result;
        }
    }
}
