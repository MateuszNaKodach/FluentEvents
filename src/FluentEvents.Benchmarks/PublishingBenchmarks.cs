using BenchmarkDotNet.Attributes;
using FluentEvents.Benchmarks.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.Benchmarks
{
    [CoreJob(baseline: true), ClrJob]
    [RPlotExporter, RankColumn]
    public class PublishingBenchmarks
    {
        private BenchmarksEventsSource _attachedEventsSource;
        private BenchmarksEventsSource _unattachedEventsSource;
        private ScopedEventRaised _event;

        [Params(PublicationTypes.Scoped, PublicationTypes.Global)]
        public PublicationTypes PublicationType { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<EventsHandlingService>();
            services.AddEventsContext<ScopedSubscriptionsEventsContext>(options => {});
            services.AddEventsContext<GlobalSubscriptionsEventsContext>(options => {});

            var serviceProvider = services.BuildServiceProvider();
            var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;

            var scopedSubscriptionsEventsContext = scopedServiceProvider.GetService<ScopedSubscriptionsEventsContext>();
            var globalSubscriptionsEventsContext = scopedServiceProvider.GetService<GlobalSubscriptionsEventsContext>();
            var eventsScope = scopedServiceProvider.GetService<EventsScope>();

            _attachedEventsSource = new BenchmarksEventsSource();
            _unattachedEventsSource = new BenchmarksEventsSource();
            _event = new ScopedEventRaised();

            var eventsContext = PublicationType == PublicationTypes.Global
                ? (EventsContext) globalSubscriptionsEventsContext
                : (EventsContext) scopedSubscriptionsEventsContext;

            eventsContext.WatchSourceEvents(_attachedEventsSource, eventsScope);

            _unattachedEventsSource.EventRaised += e => { };
        }

        [Benchmark(Baseline = true)]
        public void DirectPublication() => _unattachedEventsSource.RaiseEvent(_event);

        [Benchmark]
        public void FluentEventsPublication() => _attachedEventsSource.RaiseEvent(_event);

        public enum PublicationTypes
        {
            Scoped,
            Global
        }
    }
}