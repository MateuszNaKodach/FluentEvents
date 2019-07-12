using System;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class GlobalSubscriptionsServiceTests
    {
        private Mock<IAppServiceProvider> _appServiceProviderMock;

        private GlobalSubscriptionsService _globalSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IAppServiceProvider>(MockBehavior.Strict);

            _globalSubscriptionsService = new GlobalSubscriptionsService(_appServiceProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
        }

        [Test]
        public void AddGlobalServiceHandlerSubscription_ShouldEnqueueSubscriptionCreation()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TestService, object>();
        }

        [Test]
        public void GetGlobalSubscriptions_ShouldCreateAndReturnQueuedServiceSubscriptionCreations()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TestService, object>();

            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(TestService)))
                .Returns(new TestService())
                .Verifiable();

            var subscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(subscriptions, Has.One.Items);

            var secondCallSubscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(secondCallSubscriptions, Has.One.Items);
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
