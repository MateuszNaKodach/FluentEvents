using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentEvents.Model;
using FluentEvents.Subscriptions;
using Microsoft.CSharp.RuntimeBinder;

namespace FluentEvents.Config
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
        public IEnumerable<string> GetSelectedEventNames<TSource>(
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
                sourceModel,
                SubscriptionToDynamicActionWrapper
            );

            return subscribedHandlers.Select(x => x.EventName);
        }

        /// <inheritdoc />
        public string GetSingleSelectedEventName<TSource>(
            SourceModel sourceModel, 
            Action<TSource, object> subscriptionToDynamicAction
        )
        {
            var eventFieldNames = GetSelectedEventNames(sourceModel, subscriptionToDynamicAction);

            if (eventFieldNames.Count() > 1)
                throw new MoreThanOneEventSelectedException();

            if (!eventFieldNames.Any())
                throw new NoEventsSelectedException();

            var eventFieldName = eventFieldNames.First();

            return eventFieldName;
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
                    throw new SelectedEventReturnTypeNotSupportedException();

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
