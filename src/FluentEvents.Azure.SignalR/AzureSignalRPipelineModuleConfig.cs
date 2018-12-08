using System;
using FluentEvents.Pipelines;

namespace FluentEvents.Azure.SignalR
{
    internal class AzureSignalRPipelineModuleConfig : PipelineModuleConfig<AzureSignalRPipelineModule>
    {
        public PublicationMethod PublicationMethod { get; }
        public string HubName { get; }
        public string HubMethodName { get; }
        public Func<object, object, string[]> SubjectIdsProvider { get; }
    }
}
