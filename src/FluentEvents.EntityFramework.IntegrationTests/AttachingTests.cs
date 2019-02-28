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
        private TestEventsContext m_TestEventsContext;
        private TestDbContext m_TestDbContext;
        private IServiceProvider m_ServiceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEventsContext<TestEventsContext>(options =>
            {
                options.AttachToDbContextEntities<TestDbContext>();
            });

            var connection = Effort.DbConnectionFactory.CreateTransient();

            services.AddWithEventsAttachedTo<TestEventsContext>(trackedServices =>
            {
                trackedServices.AddScoped(x => new TestDbContext(connection));
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
                    .IsForwardedToPipeline()
                    .ThenIsPublishedToGlobalSubscriptions();
            }
        }
    }
}
