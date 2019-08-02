using System.Threading.Tasks;

namespace FluentEvents.Benchmarks.Common
{
    public class EventsHandlingService : IAsyncEventHandler<ScopedEventRaised>
    {
        public Task HandleEventAsync(ScopedEventRaised e) => Task.CompletedTask;
    }
}