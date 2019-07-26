using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;
using FluentEvents.Subscriptions;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Subscriptions
{
    [TestFixture]
    public class ServiceHandlerSubscriptionCreationTaskTests
    {
        private Mock<IRootAppServiceProvider> _appServiceProviderMock;

        private ISubscriptionCreationTask _subscriptionCreationTask;
        private ISubscriptionCreationTask _optionalSubscriptionCreationTask;

        [SetUp]
        public void SetUp()
        {
            _appServiceProviderMock = new Mock<IRootAppServiceProvider>(MockBehavior.Strict);

            _subscriptionCreationTask = new ServiceHandlerSubscriptionCreationTask<TestService, object>(false);
            _optionalSubscriptionCreationTask = new ServiceHandlerSubscriptionCreationTask<TestService, object>(true);
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
        }

        [Test]
        public void CreateSubscription_ShouldGetServicesAndCreateSubscriptions()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<TestService>)))
                .Returns(new [] { new TestService(), new TestService() })
                .Verifiable();

            var subscriptions = _subscriptionCreationTask.CreateSubscriptions(_appServiceProviderMock.Object);

            Assert.That(subscriptions, Has.Exactly(2).Items);
        }

        [Test]
        public void CreateSubscription_WhenNotOptionalNoServicesAreFound_ShouldThrow()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<TestService>)))
                .Returns(new List<TestService>())
                .Verifiable();

            Assert.That(() =>
            {
                _subscriptionCreationTask.CreateSubscriptions(_appServiceProviderMock.Object).ToArray();
            }, Throws.TypeOf<SubscribingServiceNotFoundException>());
        }

        [Test]
        public void CreateSubscription_WhenOptionalNoServicesAreFound_ShouldReturnEmptyList()
        {
            _appServiceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<TestService>)))
                .Returns(new List<TestService>())
                .Verifiable();

            var subscriptions = _optionalSubscriptionCreationTask.CreateSubscriptions(_appServiceProviderMock.Object);

            Assert.That(subscriptions, Is.Empty);
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
