using System;
using System.Collections.Generic;
using System.Text;
using FluentEvents.Config;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Config
{
    [TestFixture]
    public class SubscriptionsBuilderTests
    {
        private Mock<IGlobalSubscriptionCollection> m_GlobalSubscriptionCollection;
        private Mock<IScopedSubscriptionsService> m_ScopedSubscriptionsService;

        private SubscriptionsBuilder m_SubscriptionsBuilder;

        [SetUp]
        public void SetUp()
        {
            m_GlobalSubscriptionCollection = new Mock<IGlobalSubscriptionCollection>(MockBehavior.Strict);
            m_ScopedSubscriptionsService = new Mock<IScopedSubscriptionsService>(MockBehavior.Strict);

            m_SubscriptionsBuilder = new SubscriptionsBuilder(
                m_GlobalSubscriptionCollection.Object,
                m_ScopedSubscriptionsService.Object
            );
        }

        [Test]
        public void Service_ShouldReturnServiceConfigurator()
        {
            var serviceConfigurator = m_SubscriptionsBuilder.Service<object>();

            Assert.That(serviceConfigurator, Is.Not.Null);
            Assert.That(serviceConfigurator, Is.TypeOf<ServiceConfigurator<object>>());
        }
    }
}
