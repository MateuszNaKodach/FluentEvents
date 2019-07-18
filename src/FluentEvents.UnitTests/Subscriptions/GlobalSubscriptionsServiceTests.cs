using System;
using System.Collections.Generic;
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
        private Mock<IRootAppServiceProvider> _appServiceProviderMock;

        private GlobalSubscriptionsService _globalSubscriptionsService;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IRootAppServiceProvider>(MockBehavior.Strict);

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
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TestService, object>(false);
        }

        [Test]
        public void GetGlobalSubscriptions_ShouldCreateAndReturnQueuedServiceSubscriptionCreations()
        {
            _globalSubscriptionsService.AddGlobalServiceHandlerSubscription<TestService, object>(false);

            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<TestService>)))
                .Returns(new[] {new TestService(), new TestService()})
                .Verifiable();

            var subscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(subscriptions, Has.Exactly(2).Items);

            var secondCallSubscriptions = _globalSubscriptionsService.GetGlobalSubscriptions();
            Assert.That(secondCallSubscriptions, Has.Exactly(2).Items);
        }
        
        private class TestService : IAsyncEventHandler<object>
        {
            public Task HandleEventAsync(object e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
