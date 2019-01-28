using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    public class SubscriptionScanService : ISubscriptionScanService
    {
        private static readonly IProxyBuilder ProxyBuilder = new DefaultProxyBuilder();
        private static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions();
        private static readonly Type[] EmptyTypes = new Type[0];

        private static Type CreateClassProxyType(Type type)
            => ProxyBuilder.CreateClassProxyType(type, EmptyTypes, ProxyGenerationOptions);

        public IEnumerable<SubscribedHandler> GetSubscribedHandlers(SourceModel sourceModel, Action<object> subscriptionAction)
        {
            var type = sourceModel.ClrType;

            if (type.IsAbstract)
                type = CreateClassProxyType(type);

            var mockSource = FormatterServices.GetUninitializedObject(type);
            subscriptionAction.Invoke(mockSource);

            foreach (var eventField in sourceModel.EventFields)
            {
                var eventsHandler = (Delegate) eventField.FieldInfo.GetValue(mockSource);

                if (eventsHandler != null)
                    yield return new SubscribedHandler(eventField.EventInfo.Name, eventsHandler);
            }
        }
    }
}