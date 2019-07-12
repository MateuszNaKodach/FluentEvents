using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ServiceHandlerSubscriptionCreationTaskTests
    {
        private static readonly string _eventName = nameof(_eventName);

        private Mock<IAppServiceProvider> _appServiceProviderMock;

        private ISubscriptionCreationTask _subscriptionCreationTask;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);

            _subscriptionCreationTask = new ServiceHandlerSubscriptionCreationTask<TestService, object>();
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldGetServiceAndCreateSubscription()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            var subscription = _subscriptionCreationTask.CreateSubscription(_appServiceProviderMock.Object);

            Assert.That(subscription, Is.Not.Null);
        }

        [Test]
        public void CreateSubscription_WhenServiceIsNotFound_ShouldThrow()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(null)
                .Verifiable();

            Assert.That(() =>
            {
                _subscriptionCreationTask.CreateSubscription(_appServiceProviderMock.Object);
            }, Throws.TypeOf<SubscribingServiceNotFoundException>());
        }

        private class TestService : IEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
