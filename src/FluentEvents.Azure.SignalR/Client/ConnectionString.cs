using System;

namespace FluentEvents.Azure.SignalR.Client
{
    internal class ConnectionString
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }

        public ConnectionString(string endpoint, string accessKey)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
        }
    }
}