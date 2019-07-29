using System;
using System.Collections.Concurrent;
using FluentEvents.Infrastructure;
using FluentEvents.ServiceProviders;

namespace FluentEvents
{
    /// <summary>
    ///     The <see cref="EventsScope"/> represents the scope where entities are attached and the events
    ///     are handled or queued.
    /// </summary>
    public sealed class EventsScope : IEventsScope
    {
        private readonly IScopedAppServiceProvider _scopedAppServiceProvider;
        private readonly ConcurrentDictionary<Type, object> _features;

        /// <summary>
        ///     Creates a new <see cref="EventsScope"/>
        /// </summary>
        /// <param name="scopedAppServiceProvider">A scoped instance of the application service provider.</param>
        public EventsScope(IScopedAppServiceProvider scopedAppServiceProvider)
        {
            _scopedAppServiceProvider = scopedAppServiceProvider;
            _features = new ConcurrentDictionary<Type, object>();
        }

        T IEventsScope.GetOrAddFeature<T>(Func<IScopedAppServiceProvider, T> factory)
            => (T)_features.GetOrAdd(typeof(T), x => factory(_scopedAppServiceProvider));
    }
}
