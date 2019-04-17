using System;

namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception throw then generating an Azure SignalR Service access token with an invalid access key.
    /// </summary>
    public class InvalidConnectionStringAccessKeyException : FluentEventsAzureSignalRException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvalidConnectionStringAccessKeyException"/>.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        internal InvalidConnectionStringAccessKeyException(Exception innerException) 
            : base("The connection string access key is invalid.", innerException)
        {
            
        }
    }
}