using System.Collections.Generic;
using FluentEvents.Azure.ServiceBus.Receiving;
using FluentEvents.Azure.ServiceBus.Sending;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.ServiceBus.UnitTests
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
        public void UseAzureTopicEventSender_WithConfigureAction_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventSenderPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureTopicEventSender(x => { });
        }

        [Test]
        public void UseAzureTopicEventSender_WithConfigurationSection_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventSenderPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureTopicEventSender(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }

        [Test]
        public void UseAzureTopicEventReceiver_WithConfigureAction_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventReceiverPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureTopicEventReceiver(x => { });
        }

        [Test]
        public void UseAzureTopicEventReceiver_WithConfigurationSection_ShouldAddPlugin()
        {
            _fluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventReceiverPlugin>()))
                .Verifiable();

            _fluentEventPluginOptionsMock.Object.UseAzureTopicEventReceiver(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }
    }
}
