using System;
using FluentEvents.Config;
using FluentEvents.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentEvents.UnitTests
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

            Action<EventsContextOptions> onConfiguring = x => { isOnConfiguringInvoked = true; };
            Action<PipelinesBuilder> onBuildingPipelines = x => { isOnBuildingPipelinesInvoked = true; };
            Action<SubscriptionsBuilder> onBuildingSubscriptions = x => { isOnBuildingSubscriptionsInvoked = true; };

            var internalEventsContext = new InternalEventsContext(
                _options,
                onConfiguring,
                onBuildingPipelines,
                onBuildingSubscriptions,
                _appServiceProvider.Object
            );

            Assert.That(isOnConfiguringInvoked, Is.True);
            Assert.That(isOnBuildingPipelinesInvoked, Is.True);
            Assert.That(isOnBuildingSubscriptionsInvoked, Is.True);

            Assert.That(internalEventsContext, Has.Property(nameof(internalEventsContext.InternalServiceProvider)).Not.Null);
        }
    }
}
