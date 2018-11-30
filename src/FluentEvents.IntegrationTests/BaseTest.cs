using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FluentEvents.IntegrationTests
{
    public class BaseTest<TContext> where TContext : EventsContext
    {
        public const string TestEntityId = "TestEntityId";
        public const string TestValue = "TestValue";

        public TContext Context { get; set; }
        public TestEntity Entity { get; set; }
        public SubscribingService SubscribingService { get; set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public EventsScope Scope { get; set; }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
       
            services.AddEventsContext<TContext>(options =>
            {

            });
          
            services.AddScoped<SubscribingService>();
            ServiceProvider = services.BuildServiceProvider();

            Entity = new TestEntity {Id = TestEntityId};

            var serviceScope = ServiceProvider.CreateScope();
            SubscribingService = serviceScope.ServiceProvider.GetRequiredService<SubscribingService>();
            Context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
            Scope = serviceScope.ServiceProvider.GetRequiredService<EventsScope>();

            Context.Attach(Entity, Scope);
        }

    }
}