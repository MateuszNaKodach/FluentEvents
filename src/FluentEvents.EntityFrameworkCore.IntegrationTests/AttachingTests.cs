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
        private TestEventsContext m_TestEventsContext;
        private TestDbContext m_TestDbContext;
        private ServiceProvider m_ServiceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.AttachToDbContextEntities<TestDbContext>();
            });

            services.AddWithEventsAttachedTo<TestEventsContext>(trackedServices =>
            {
                trackedServices.AddDbContext<TestDbContext>(options =>
                {
                    options.UseInMemoryDatabase("test");
                });
            });
         
            m_ServiceProvider = services.BuildServiceProvider();

            using (var serviceScope = m_ServiceProvider.CreateScope())
            {
                m_TestEventsContext = serviceScope.ServiceProvider.GetRequiredService<TestEventsContext>();
                m_TestDbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

                m_TestDbContext.TestEntities.Add(new TestEntity());
                m_TestDbContext.SaveChanges();
            }
        }

        [Test]
        public void AttachTest()
        {
            TestEventArgs testEventArgs = null;
            m_TestEventsContext.SubscribeGloballyTo<TestEntity>(testEntity =>
            {
                testEntity.Test += (sender, args) =>
                {
                    testEventArgs = args;
                };
            });

            using (var serviceScope = m_ServiceProvider.CreateScope())
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
                    .IsForwardedToPipeline()
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
