using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using Microsoft.CSharp.RuntimeBinder;

namespace FluentEvents.Utils
{
    /// <inheritdoc />
    public class EventSelectionService : IEventSelectionService
    {
        private readonly ISubscriptionScanService _subscriptionScanService;

        /// <summary>
        ///     This API supports the FluentEvents infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public EventSelectionService(ISubscriptionScanService subscriptionScanService)
        {
            _subscriptionScanService = subscriptionScanService;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSelectedEvent<TSource>(
            SourceModel sourceModel,
            Action<TSource, object> subscriptionToDynamicAction
        )
        {
            void WrappingSubscriptionToDynamicAction(object x)
            {
                try
                {
                    subscriptionToDynamicAction((TSource)x, new DynamicEventHandler());
                }
                catch (RuntimeBinderException e)
                {
                    throw new InvalidEventSelectionException(e);
                }
            }

            var subscribedHandlers = _subscriptionScanService.GetSubscribedHandlers(
                sourceModel.ClrType,
                sourceModel.ClrTypeFieldInfos,
                WrappingSubscriptionToDynamicAction
            );

            return subscribedHandlers.Select(x => x.EventName);
        }

        private class DynamicEventHandler : DynamicObject
        {
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                if (!binder.Type.IsSubclassOf(typeof(Delegate)))
                {
                    result = null;
                    return false;
                }

                result = CreateEventHandler(binder.Type);
                return true;
            }

            private Delegate CreateEventHandler(Type type)
            {
                var eventHandlerParameters = type
                    .GetMethod(nameof(EventHandler.Invoke))
                    .GetParameters()
                    .Select(parameter => Expression.Parameter(parameter.ParameterType))
                    .ToArray();

                var handler = Expression.Lambda(
                        type,
                        Expression.Empty(),
                        eventHandlerParameters
                    )
                    .Compile();

                return handler;
            }
        }
    }
}
