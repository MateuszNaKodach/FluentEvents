using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        public IEnumerable<string> GetSelectedEvents<TSource>(
            SourceModel sourceModel,
            Action<TSource, object> subscriptionToDynamicAction
        )
        {
            void SubscriptionToDynamicActionWrapper(object x)
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
                SubscriptionToDynamicActionWrapper
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
                var invokeMethod = type
                    .GetMethod(nameof(EventHandler.Invoke));

                var eventHandlerParameters = invokeMethod
                    .GetParameters()
                    .Select(parameter => Expression.Parameter(parameter.ParameterType))
                    .ToArray();

                var returnType = invokeMethod.ReturnType;
                Expression body;
                
                if (returnType == typeof(Task))
                    body = Expression.Constant(Task.CompletedTask);
                else if (returnType == typeof(void))
                    body = Expression.Empty();
                else
                    throw new SelectedEventHasUnsupportedReturnTypeException();

                var handler = Expression.Lambda(
                        type,
                        body,
                        eventHandlerParameters
                    )
                    .Compile();

                return handler;
            }
        }
    }
}
