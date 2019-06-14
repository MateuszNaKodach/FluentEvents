using System;
using System.Collections.Generic;
using FluentEvents.Model;
using FluentEvents.Subscriptions;

namespace FluentEvents.Utils
{
    /// <summary>
    ///     This API supports the FluentEvents infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IEventSelectionService
    {
        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        IEnumerable<string> GetSelectedEvents<TSource>(
            SourceModel sourceModel,
            Action<TSource, object> subscriptionToDynamicAction
        );
    }
}