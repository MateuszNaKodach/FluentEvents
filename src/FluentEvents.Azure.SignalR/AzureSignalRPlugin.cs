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
        private readonly IConfiguration _configuration;
        private readonly Action<AzureSignalRServiceConfig> _configureAction;
        private readonly Action<IHttpClientBuilder> _httpClientBuilderAction;

        public AzureSignalRPlugin(
            Action<AzureSignalRServiceConfig> configureAction,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            _configureAction = configureAction ?? throw new ArgumentNullException(nameof(configureAction));
            _httpClientBuilderAction = httpClientBuilderAction;
        }

        public AzureSignalRPlugin(
            IConfiguration configuration,
            Action<IHttpClientBuilder> httpClientBuilderAction
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientBuilderAction = httpClientBuilderAction;
        }

        public void ApplyServices(IServiceCollection services)
        {
            if (_configureAction != null)
                services.Configure(_configureAction);
            else
                services.Configure<AzureSignalRServiceConfig>(_configuration);

            var httpClientBuilder = services.AddHttpClient<IAzureSignalRHttpClient, AzureSignalRHttpClient>();
            _httpClientBuilderAction?.Invoke(httpClientBuilder);

            services.AddSingleton<IValidableConfig>(x =>
                x.GetRequiredService<IOptions<AzureSignalRServiceConfig>>().Value
            );
            services.AddSingleton<AzureSignalRPipelineModule>();            
            services.AddSingleton<IEventSendingService, EventSendingService>();
            services.AddSingleton<IUrlProvider, UrlProvider>();
            services.AddSingleton<IHttpRequestFactory, HttpRequestFactory>();
            services.AddSingleton<IAccessTokensService, AccessTokensService>();
        }
    }
}
