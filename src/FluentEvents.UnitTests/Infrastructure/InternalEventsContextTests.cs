using System;
using FluentEvents.Configuration;
using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Infrastructure
{
    [TestFixture]
    public class InternalEventsContextTests
    {
        private EventsContextOptions _options;
        private Mock<IRootAppServiceProvider> _appServiceProviderMock;
        private Mock<IEventsContext> _eventsContextMock;

        [SetUp]
        public void SetUp()
        {
            _options = new EventsContextOptions();
            _appServiceProviderMock = new Mock<IRootAppServiceProvider>(MockBehavior.Strict);
            _eventsContextMock = new Mock<IEventsContext>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProviderMock.Verify();
        }
        
        [Test]
        public void Ctor_ShouldCallActionsAndCreateInternalServiceProvider()
        {
            var isOnConfiguringInvoked = false;
            var isOnBuildingPipelinesInvoked = false;
            var isOnBuildingSubscriptionsInvoked = false;

            void OnConfiguring(EventsContextOptions x) => isOnConfiguringInvoked = true;

            void OnBuildingPipelines(IPipelinesBuilder x) => isOnBuildingPipelinesInvoked = true;

            void OnBuildingSubscriptions(SubscriptionsBuilder x) => isOnBuildingSubscriptionsInvoked = true;

            var internalEventsContext = new InternalEventsContext(
                _options,
                OnConfiguring,
                OnBuildingPipelines,
                OnBuildingSubscriptions,
                _appServiceProviderMock.Object,
                _eventsContextMock.Object
            );

            Assert.That(isOnConfiguringInvoked, Is.True);
            Assert.That(isOnBuildingPipelinesInvoked, Is.True);
            Assert.That(isOnBuildingSubscriptionsInvoked, Is.True);

            Assert.That(
                internalEventsContext,
                Has.Property(nameof(internalEventsContext.InternalServiceProvider)).Not.Null
            );
        }

        [Test]
        public void Dispose_ShouldDisposeInternalServiceProvider()
        {
            void OnConfiguring(EventsContextOptions x)
            {
            }

            void OnBuildingPipelines(IPipelinesBuilder x)
            {
            }

            void OnBuildingSubscriptions(SubscriptionsBuilder x)
            {
            }

            var internalEventsContext = new InternalEventsContext(
                _options,
                OnConfiguring,
                OnBuildingPipelines,
                OnBuildingSubscriptions,
                _appServiceProviderMock.Object,
                _eventsContextMock.Object
            );

            internalEventsContext.Dispose();

            Assert.That(() =>
            {
                internalEventsContext.InternalServiceProvider.GetService(typeof(object));
            }, Throws.TypeOf<ObjectDisposedException>());
        }
    }
}
