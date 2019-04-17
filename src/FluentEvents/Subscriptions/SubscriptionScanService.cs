using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using FluentEvents.Model;

namespace FluentEvents.Subscriptions
{
    /// <inheritdoc />
    public class SubscriptionScanService : ISubscriptionScanService
    {
        private static readonly IProxyBuilder ProxyBuilder = new DefaultProxyBuilder();
        private static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions();

        private static Type CreateClassProxyType(Type type)
            => ProxyBuilder.CreateClassProxyType(type, Type.EmptyTypes, ProxyGenerationOptions);

        /// <inheritdoc />
        public IEnumerable<SubscribedHandler> GetSubscribedHandlers(
            Type sourceType, 
            IEnumerable<FieldInfo> fieldInfos, 
            Action<object> subscriptionAction
        )
        {
            if (sourceType.IsAbstract)
                sourceType = CreateClassProxyType(sourceType);

            var mockSource = FormatterServices.GetUninitializedObject(sourceType);
            subscriptionAction.Invoke(mockSource);

            foreach (var fieldInfo in fieldInfos)
            {
                var eventsHandler = (Delegate) fieldInfo.GetValue(mockSource);

                if (eventsHandler != null)
                    yield return new SubscribedHandler(fieldInfo.Name, eventsHandler);
            }
        }
    }
}