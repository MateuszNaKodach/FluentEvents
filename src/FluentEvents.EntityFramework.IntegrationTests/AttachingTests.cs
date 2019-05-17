using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.EntityFramework.IntegrationTests
{
    [TestFixture]
    public class AttachingTests
    {
        private TestEventsContext _testEventsContext;
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
         
            _serviceProvider = services.BuildServiceProvider();

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                _testEventsContext = serviceScope.ServiceProvider.GetRequiredService<TestEventsContext>();
                _testDbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

                _testDbContext.TestEntities.Add(new TestEntity());
                _testDbContext.SaveChanges();
            }
        }

        [Test]
        public void AttachFromQueryResultTest()
        {
            TestEventArgs testEventArgs = null;
            _testEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    testEventArgs = args;
                };
            });

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var testDbContext = serviceScope.ServiceProvider.GetService<TestDbContext>();
                var testEntity = testDbContext.TestEntities.First();
                testEntity.RaiseEvent("");
            }
            
            Assert.That(testEventArgs, Is.Not.Null);
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
            protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
            {
                pipelinesBuilder
                    .Event<TestEntity, TestEventArgs>(nameof(TestEntity.Test))
                    .IsWatched()
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
