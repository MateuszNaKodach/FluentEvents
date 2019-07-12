using System;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModuleConfig
    {
        public PublicationMethod PublicationMethod { get; set; }
        public string HubName { get; set; }
        public string HubMethodName { get; set; }
        public Func<object, string[]> ReceiverIdsProviderAction { get; set; }
    }
}
