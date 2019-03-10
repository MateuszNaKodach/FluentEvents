using System;
using System.Collections.Generic;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class FluentEventsPluginOptionsExtensionsTests
    {
        private Mock<IFluentEventsPluginOptions> m_FluentEventsPluginOptionsMock;

        [SetUp]
        public void SetUp()
        {
            m_FluentEventsPluginOptionsMock = new Mock<IFluentEventsPluginOptions>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            m_FluentEventsPluginOptionsMock.Verify();
        }

        [Test]
        public void UseAzureSignalRService_WithConfigureAction_ShouldAddPlugin([Values] bool isHttpClientBuilderActionNull)
        {
            var httpClientBuilderAction = isHttpClientBuilderActionNull
                ? (Action<IHttpClientBuilder>) null
                : builder => { };

            m_FluentEventsPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureSignalRPlugin>()))
                .Verifiable();

            m_FluentEventsPluginOptionsMock.Object.UseAzureSignalRService(options => { }, httpClientBuilderAction);
        }

        [Test]
        public void UseAzureSignalRService_WithConfiguration_ShouldAddPlugin([Values] bool isHttpClientBuilderActionNull)
        {
            var httpClientBuilderAction = isHttpClientBuilderActionNull
                ? (Action<IHttpClientBuilder>)null
                : builder => { };

            m_FluentEventsPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureSignalRPlugin>()))
                .Verifiable();

            m_FluentEventsPluginOptionsMock.Object.UseAzureSignalRService(
                new ConfigurationRoot(new List<IConfigurationProvider>()),
                httpClientBuilderAction
            );
        }

        [Test]
        public void UseAzureSignalRService_WithNullConfigureAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_FluentEventsPluginOptionsMock.Object.UseAzureSignalRService((Action<AzureSignalRServiceConfig>)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void UseAzureSignalRService_WithNullConfiguration_ShouldThrow()
        {
            Assert.That(() =>
            {
                m_FluentEventsPluginOptionsMock.Object.UseAzureSignalRService((IConfiguration)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
