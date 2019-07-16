using System;
using System.Collections.Generic;
using FluentEvents.Azure.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FluentEvents.Azure.SignalR.UnitTests
{
    [TestFixture]
    public class AzureSignalRPluginTests
    {
        [Test]
        public void ApplyServices_ShouldAddServices(
            [Values] bool useConfigureAction,
            [Values] bool isHttpClientBuilderActionNull
        )
        {
            var isHttpClientBuilderActionInvoked = false;
            Action<IHttpClientBuilder> httpClientBuilderAction = null;
            if (!isHttpClientBuilderActionNull)
                httpClientBuilderAction = x => { isHttpClientBuilderActionInvoked = true; };

            var azureSignalRPlugin = useConfigureAction 
                ? new AzureSignalRPlugin(new ConfigurationRoot(new List<IConfigurationProvider>()), httpClientBuilderAction)
                : new AzureSignalRPlugin(options => { }, httpClientBuilderAction);

            var services = new ServiceCollection();
            azureSignalRPlugin.ApplyServices(services);

             Assert.That(
                services,
                Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                    .EqualTo(typeof(IConfigureOptions<AzureSignalRServiceOptions>))
            );

             Assert.That(
                 services,
                 Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                     .EqualTo(typeof(IValidateOptions<AzureSignalRServiceOptions>))
             );

             Assert.That(
                 services,
                 Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                     .EqualTo(typeof(AzureSignalRPipelineModule))
             );

             Assert.That(
                 services,
                 Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                     .EqualTo(typeof(IUrlProvider))
             );

             Assert.That(
                 services,
                 Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                     .EqualTo(typeof(IHttpRequestFactory))
             );

             Assert.That(
                 services,
                 Has.One.Items.With.Property(nameof(ServiceDescriptor.ServiceType))
                     .EqualTo(typeof(IAccessTokensService))
             );

            if (!isHttpClientBuilderActionNull)
                 Assert.That(isHttpClientBuilderActionInvoked, Is.True);
        }
    }
}
