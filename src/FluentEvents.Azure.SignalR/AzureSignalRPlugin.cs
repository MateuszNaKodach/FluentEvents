using System;
using FluentEvents.Azure.SignalR.Client;
using FluentEvents.Infrastructure;
using FluentEvents.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPlugin : IFluentEventsPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly Action<AzureSignalRClientConfig> m_ConfigureOptions;
        private readonly Action<IHttpClientBuilder> m_HttpClientBuilderAction;

        public AzureSignalRPlugin(
            Action<AzureSignalRClientConfig> configureAction,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            m_ConfigureOptions = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
            m_HttpClientBuilderAction = httpClientBuilderAction;
        }

        public AzureSignalRPlugin(
            IConfiguration configureOptions,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            m_Configuration = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
            m_HttpClientBuilderAction = httpClientBuilderAction;
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureOptions != null)
                services.Configure(m_ConfigureOptions);
            else
                services.Configure<AzureSignalRClientConfig>(m_Configuration);

            var httpClientBuilder = services.AddHttpClient<IAzureSignalRClient, AzureSignalRClient>();
            m_HttpClientBuilderAction?.Invoke(httpClientBuilder);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureSignalRClientConfig>>().Value
            );
            services.AddSingleton<AzureSignalRPipelineModule>();            
            services.AddSingleton<IUrlProvider, UrlProvider>();
            services.AddSingleton<IConnectionStringBuilder, ConnectionStringBuilder>();
            services.AddSingleton<IHttpRequestFactory, HttpRequestFactory>();
            services.AddSingleton<IAccessTokensService, AccessTokensService>();
        }
    }
}
