using System;
using System.ComponentModel;

namespace FluentEvents.Infrastructure
{
    /// <summary>
    ///     The <see cref="IEventsScope"/> represents the scope where entities are attached and the events
    ///     are handled or queued.
    /// </summary>
    public interface IEventsScope
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        T GetOrAddFeature<T>(Func<IScopedAppServiceProvider, T> factory);
    }
}