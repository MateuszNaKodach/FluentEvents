namespace FluentEvents.Benchmarks.Common
{
    public class BenchmarksEventsSource
    {
        public event EventPublisher<ScopedEventRaised> EventRaised;

        public void RaiseEvent(ScopedEventRaised e)
        {
            EventRaised?.Invoke(e);
        }
    }
}
