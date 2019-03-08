using FluentEvents.Infrastructure;

namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     The configuration of the Azure SignalR Service plugin.
    /// </summary>
    public class AzureSignalRClientConfig : IValidableConfig
    {
        private string m_ConnectionString;

        /// <summary>
        ///     The connection string of the Azure SignalR Service.
        /// </summary>
        public string ConnectionString
        {
            get => m_ConnectionString;
            set => m_ConnectionString = ConnectionStringValidator.Validate(value);
        }

        void IValidableConfig.Validate()
        {
            if (ConnectionString == null)
                throw new ConnectionStringIsNullException();
        }
    }
}