using System.Threading.Tasks;
using FluentEvents.Pipelines;

namespace FluentEvents.Transmission
{
    public interface IEventSender
    {
        Task SendAsync(PipelineEvent pipelineEvent);
    }
}