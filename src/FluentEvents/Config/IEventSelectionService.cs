using System;
using System.Collections.Generic;
using FluentEvents.Model;

namespace FluentEvents.Config
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
        IEnumerable<string> GetSelectedEventNames<TSource>(
            SourceModel sourceModel,
            Action<TSource, object> subscriptionToDynamicAction
        );

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        string GetSingleSelectedEventName<TSource>(
            SourceModel sourceModel,
            Action<TSource, object> subscriptionToDynamicAction
        );
    }
}