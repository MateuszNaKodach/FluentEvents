using System.Linq;
using FluentEvents.Config;
using FluentEvents.IntegrationTests.Common;
using FluentEvents.Pipelines.Publication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.EntityFrameworkCore.IntegrationTests
{
    [TestFixture]
    public class AttachingTests
    {
        private TestEventsContext _testEventsContext;
        private TestDbContext _testDbContext;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.AttachToDbContextEntities<TestDbContext>();
            });

            services.AddWithEventsAttachedTo<TestEventsContext>(() =>
            {
                services.AddDbContext<TestDbContext>(options =>
                {
                    options.UseInMemoryDatabase("test");
                });
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
            public DbSet<TestEntity> TestEntities { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options) 
                : base(options)
            {
                
            }
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
