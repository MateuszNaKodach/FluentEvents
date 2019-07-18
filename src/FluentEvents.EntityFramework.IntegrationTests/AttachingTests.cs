using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.EntityFramework.IntegrationTests
{
    [TestFixture]
    public class AttachingTests
    {
        private TestDbContext _testDbContext;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.AttachToDbContextEntities<TestDbContext>();
            });

            var connection = Effort.DbConnectionFactory.CreateTransient();

            services.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                services.AddScoped(x => new TestDbContext(connection));
            });
            services.AddSingleton<SubscribingService>();

            _serviceProvider = services.BuildServiceProvider();

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                _testDbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

                _testDbContext.TestEntities.Add(new TestEntity());
                _testDbContext.SaveChanges();
            }
        }

        [Test]
        public void AttachFromQueryResultTest()
        {
            var subscribingService = _serviceProvider.GetRequiredService<SubscribingService>();

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var testDbContext = serviceScope.ServiceProvider.GetService<TestDbContext>();
                var testEntity = testDbContext.TestEntities.First();
                testEntity.RaiseEvent("");
            }

            Assert.That(subscribingService, Has.Property(nameof(SubscribingService.TestEvents)).With.One.Items);
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<TestEntity> TestEntities { get; set; }
        }

        private class TestEventsContext : EventsContext
        {
            protected override void OnBuildingSubscriptions(SubscriptionsBuilder subscriptionsBuilder)
            {
                subscriptionsBuilder
                    .ServiceHandler<SubscribingService, TestEvent>()
                    .HasGlobalSubscription();
            }

            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEvent>()
                    .IsPiped()
                    .ThenIsPublishedToGlobalSubscriptions();
            }

            public TestEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) 
                : base(options, rootAppServiceProvider)
            {
            }
        }
    }
}
