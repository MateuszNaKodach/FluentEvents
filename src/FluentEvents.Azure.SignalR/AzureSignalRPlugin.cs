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
        private readonly Action<AzureSignalRServiceConfig> m_ConfigureAction;
        private readonly Action<IHttpClientBuilder> m_HttpClientBuilderAction;

        public AzureSignalRPlugin(
            Action<AzureSignalRServiceConfig> configureAction,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            m_ConfigureAction = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
            m_HttpClientBuilderAction = httpClientBuilderAction;
        }

        public AzureSignalRPlugin(
            IConfiguration configuration,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            m_HttpClientBuilderAction = httpClientBuilderAction;
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (m_ConfigureAction != null)
                services.Configure(m_ConfigureAction);
            else
                services.Configure<AzureSignalRServiceConfig>(m_Configuration);

            var httpClientBuilder = services.AddHttpClient<IAzureSignalRClient, AzureSignalRClient>();
            m_HttpClientBuilderAction?.Invoke(httpClientBuilder);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureSignalRServiceConfig>>().Value
            );
            services.AddSingleton<AzureSignalRPipelineModule>();            
            services.AddSingleton<IUrlProvider, UrlProvider>();
            services.AddSingleton<IHttpRequestFactory, HttpRequestFactory>();
            services.AddSingleton<IAccessTokensService, AccessTokensService>();
        }
    }
}
