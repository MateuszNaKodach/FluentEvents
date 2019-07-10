using System.Collections.Generic;
using FluentEvents.Azure.ServiceBus.Queues;
using FluentEvents.Azure.ServiceBus.Queues.Receiving;
using FluentEvents.Azure.ServiceBus.Queues.Sending;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests.Queues
{
    [TestFixture]
    public class FluentEventsPluginOptionsExtensionsTests
    {
        private Mock<IFluentEventsPluginOptions> _fluentEventPluginOptionsMock;

        [SetUp]
        public void SetUp()
        {
            _fluentEventPluginOptionsMock = new Mock<IFluentEventsPluginOptions>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _fluentEventPluginOptionsMock.Verify();
        }

        [Test]
        public void UseAzureQueueEventSender_WithConfigureAction_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureQueueEventSenderPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureQueueEventSender(x => { });
        }

        [Test]
        public void UseAzureQueueEventSender_WithConfigurationSection_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureQueueEventSenderPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureQueueEventSender(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }

        [Test]
        public void UseAzureQueueEventReceiver_WithConfigureAction_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureQueueEventReceiverPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureQueueEventReceiver(x => { });
        }

        [Test]
        public void UseAzureQueueEventReceiver_WithConfigurationSection_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureQueueEventReceiverPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureQueueEventReceiver(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }
    }
}
