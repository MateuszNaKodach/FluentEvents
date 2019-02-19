using System;

namespace FluentEvents.Config
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public abstract class BuilderBase
    {
        /// <summary>
        ///     This API supports the FluentEvents Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected EventsContext EventsContext { get; }
        /// <summary>
        ///     This API supports the FluentEvents Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        ///     This API supports the FluentEvents Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected BuilderBase(EventsContext eventsContext, IServiceProvider serviceProvider)
        {
            EventsContext = eventsContext;
            ServiceProvider = serviceProvider;
        }
    }
}