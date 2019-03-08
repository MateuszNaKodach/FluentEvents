using System;
using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.SignalR.Client
{
    /// <inheritdoc cref="IAzureSignalRClientConfig" />
    public class AzureSignalRClientConfig : IAzureSignalRClientConfig, IValidableConfig
    {
        /// <inheritdoc />
        public string ConnectionString { get; set; }

        void IValidableConfig.Validate()
        {
            if (ConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}