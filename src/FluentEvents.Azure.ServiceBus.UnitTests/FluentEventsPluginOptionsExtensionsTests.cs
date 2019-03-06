using System;
using System.Collections.Generic;
using System.Text;
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
        private Mock<IFluentEventsPluginOptions> m_FluentEventPluginOptionsMock;

        [SetUp]
        public void SetUp()
        {
            m_FluentEventPluginOptionsMock = new Mock<IFluentEventsPluginOptions>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            m_FluentEventPluginOptionsMock.Verify();
        }

        [Test]
        public void UseAzureTopicEventSender_WithConfigureAction_ShouldAddPlugin()
        {
            m_FluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventSenderPlugin>()))
                .Verifiable();

            m_FluentEventPluginOptionsMock.Object.UseAzureTopicEventSender(x => { });
        }

        [Test]
        public void UseAzureTopicEventSender_WithConfigurationSection_ShouldAddPlugin()
        {
            m_FluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventSenderPlugin>()))
                .Verifiable();

            m_FluentEventPluginOptionsMock.Object.UseAzureTopicEventSender(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }

        [Test]
        public void UseAzureTopicEventReceiver_WithConfigureAction_ShouldAddPlugin()
        {
            m_FluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventReceiverPlugin>()))
                .Verifiable();

            m_FluentEventPluginOptionsMock.Object.UseAzureTopicEventReceiver(x => { });
        }

        [Test]
        public void UseAzureTopicEventReceiver_WithConfigurationSection_ShouldAddPlugin()
        {
            m_FluentEventPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureTopicEventReceiverPlugin>()))
                .Verifiable();

            m_FluentEventPluginOptionsMock.Object.UseAzureTopicEventReceiver(
                new ConfigurationRoot(new List<IConfigurationProvider>())
            );
        }
    }
}
