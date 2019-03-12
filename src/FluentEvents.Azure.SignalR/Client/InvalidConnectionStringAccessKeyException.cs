using System;

namespace FluentEvents.Azure.SignalR.Client
{
    /// <summary>
    ///     An exception throw then generating an Azure SignalR Service access token with an invalid access key.
    /// </summary>
    public class InvalidConnectionStringAccessKeyException : FluentEventsException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="InvalidConnectionStringAccessKeyException"/>.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public InvalidConnectionStringAccessKeyException(Exception innerException) 
            : base(null, innerException)
        {
            
        }
    }
}