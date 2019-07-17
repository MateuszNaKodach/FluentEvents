using System;
using System.Collections.Concurrent;
using FluentEvents.Infrastructure;

namespace FluentEvents
{
    /// <summary>
    ///      Allows <see cref="EventsContext"/>s to persist their configuration when they are disposed.
    /// </summary>
    public sealed class EventsContextsRoot : IDisposable
    {
        private readonly ConcurrentDictionary<(Type, EventsContextOptions), InternalEventsContext> _internalEventsContexts;
        private bool _isDisposed;

        internal IAppServiceProvider AppServiceProvider { get; }

        /// <summary>
        ///     Creates a new <see cref="EventsContextsRoot"/>.
        /// </summary>
        public EventsContextsRoot(IAppServiceProvider appServiceProvider)
        {
            AppServiceProvider = appServiceProvider;
            _internalEventsContexts = new ConcurrentDictionary<(Type, EventsContextOptions), InternalEventsContext>();
        }

        internal InternalEventsContext GetOrCreateContext(
            Type type,
            EventsContextOptions options,
            Func<InternalEventsContext> factory
        )
        {
            CheckDisposed();

            return _internalEventsContexts.GetOrAdd((type, options), x => factory());
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventsContextsRoot));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _isDisposed = true;

            while (_internalEventsContexts.Count > 0)
            {
                foreach (var key in _internalEventsContexts.Keys)
                {
                    if (_internalEventsContexts.TryRemove(key, out var internalEventsContext))
                        internalEventsContext.Dispose();
                }
            }
        }
    }
}
