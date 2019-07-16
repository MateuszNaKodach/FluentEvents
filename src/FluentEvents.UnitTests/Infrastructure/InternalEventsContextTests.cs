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
        private Mock<IAppServiceProvider> _appServiceProvider;

        [SetUp]
        public void SetUp()
        {
            _options = new EventsContextOptions();
            _appServiceProvider = new Mock<IAppServiceProvider>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _appServiceProvider.Verify();
        }
        
        [Test]
        public void Ctor_ShouldCallActionsAndCreateInternalServiceProvider()
        {
            var isOnConfiguringInvoked = false;
            var isOnBuildingPipelinesInvoked = false;
            var isOnBuildingSubscriptionsInvoked = false;

            void OnConfiguring(EventsContextOptions x) => isOnConfiguringInvoked = true;

            void OnBuildingPipelines(PipelinesBuilder x) => isOnBuildingPipelinesInvoked = true;

            void OnBuildingSubscriptions(SubscriptionsBuilder x) => isOnBuildingSubscriptionsInvoked = true;

            var internalEventsContext = new InternalEventsContext(
                _options,
                OnConfiguring,
                OnBuildingPipelines,
                OnBuildingSubscriptions,
                _appServiceProvider.Object
            );

            Assert.That(isOnConfiguringInvoked, Is.True);
            Assert.That(isOnBuildingPipelinesInvoked, Is.True);
            Assert.That(isOnBuildingSubscriptionsInvoked, Is.True);

            Assert.That(
                internalEventsContext,
                Has.Property(nameof(internalEventsContext.InternalServiceProvider)).Not.Null
            );
        }
    }
}
