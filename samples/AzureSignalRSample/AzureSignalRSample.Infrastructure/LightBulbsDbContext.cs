using System.Threading;
using System.Threading.Tasks;
using AzureSignalRSample.Application;
using AzureSignalRSample.Domain;
using FluentEvents;
using Microsoft.EntityFrameworkCore;

namespace AzureSignalRSample.Infrastructure
{
    public class LightBulbsDbContext : DbContext, ILightBulbsTransaction
    {
        private readonly LightBulbsEventsContext _lightBulbsEventsContext;
        private readonly EventsScope _eventsScope;

        public LightBulbsDbContext(
            LightBulbsEventsContext lightBulbsEventsContext,
            EventsScope eventsScope,
            DbContextOptions<LightBulbsDbContext> options
        ) : base(options)
        {
            _lightBulbsEventsContext = lightBulbsEventsContext;
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

            await _lightBulbsEventsContext.ProcessQueuedEventsAsync(_eventsScope, LightBulbsEventsContext.AfterSaveChangesQueueName);

            return result;
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync();
        }
    }
}
