using System;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class SubscriptionCreationTaskTests
    {
        private Mock<IAppServiceProvider> m_AppServiceProviderMock;
        private Mock<ISubscriptionsFactory> m_SubscriptionsFactoryMock;
        private Subscription m_Subscription;

        private ISubscriptionCreationTask m_SubscriptionCreationTask;

        [SetUp]
        public void SetUp()
        {
            m_AppServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);
            m_SubscriptionsFactoryMock = new Mock<ISubscriptionsFactory>(MockBehavior.Strict);
            m_Subscription = new Subscription(typeof(object));

            m_SubscriptionCreationTask = new SubscriptionCreationTask<TestService, object>(
                (o, o1) => { },
                m_SubscriptionsFactoryMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            m_AppServiceProviderMock.Verify();
            m_SubscriptionsFactoryMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldGetServiceAndCreateSubscription()
        {
            m_AppServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            m_SubscriptionsFactoryMock
                .Setup(x => x.CreateSubscription(It.IsAny<Action<object>>()))
                .Returns(m_Subscription)
                .Verifiable();

            var subscription = m_SubscriptionCreationTask.CreateSubscription(m_AppServiceProviderMock.Object);

            Assert.That(subscription, Is.Not.Null);
        }

        public class TestService { }
    }
}
