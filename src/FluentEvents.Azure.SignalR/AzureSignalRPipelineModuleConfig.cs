using System;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModuleConfig
    {
        public PublicationMethod PublicationMethod { get; }
        public string HubName { get; }
        public string HubMethodName { get; }
        public Func<object, object, string[]> SubjectIdsProvider { get; }
    }
}
