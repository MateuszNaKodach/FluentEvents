using System;
using System.Collections.Generic;
using FluentEvents.Configuration;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class EventsContextOptionsExtensionsTests
    {
        private Mock<IEventsContextOptions> _fluentEventsPluginOptionsMock;

        [SetUp]
        public void SetUp()
        {
            _fluentEventsPluginOptionsMock = new Mock<IEventsContextOptions>(MockBehavior.Strict);
        }

        [TearDown]
        public void TearDown()
        {
            _fluentEventsPluginOptionsMock.Verify();
        }

        [Test]
        public void UseAzureSignalRService_WithConfigureAction_ShouldAddPlugin([Values] bool isHttpClientBuilderActionNull)
        {
            var httpClientBuilderAction = isHttpClientBuilderActionNull
                ? (Action<IHttpClientBuilder>) null
                : builder => { };

            _fluentEventsPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureSignalRPlugin>()))
                .Verifiable();

            _fluentEventsPluginOptionsMock.Object.UseAzureSignalRService(options => { }, httpClientBuilderAction);
        }

        [Test]
        public void UseAzureSignalRService_WithConfiguration_ShouldAddPlugin([Values] bool isHttpClientBuilderActionNull)
        {
            var httpClientBuilderAction = isHttpClientBuilderActionNull
                ? (Action<IHttpClientBuilder>)null
                : builder => { };

            _fluentEventsPluginOptionsMock
                .Setup(x => x.AddPlugin(It.IsAny<AzureSignalRPlugin>()))
                .Verifiable();

            _fluentEventsPluginOptionsMock.Object.UseAzureSignalRService(
                new ConfigurationRoot(new List<IConfigurationProvider>()),
                httpClientBuilderAction
            );
        }

        [Test]
        public void UseAzureSignalRService_WithNullConfigureAction_ShouldThrow()
        {
            Assert.That(() =>
            {
                _fluentEventsPluginOptionsMock.Object.UseAzureSignalRService((Action<AzureSignalRServiceOptions>)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void UseAzureSignalRService_WithNullConfiguration_ShouldThrow()
        {
            Assert.That(() =>
            {
                _fluentEventsPluginOptionsMock.Object.UseAzureSignalRService((IConfiguration)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
