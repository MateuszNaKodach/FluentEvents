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
        private static readonly IProxyBuilder _proxyBuilder = new DefaultProxyBuilder();
        private static readonly ProxyGenerationOptions _proxyGenerationOptions = new ProxyGenerationOptions();

        private static Type CreateClassProxyType(Type type)
            => _proxyBuilder.CreateClassProxyType(type, Type.EmptyTypes, _proxyGenerationOptions);

        /// <inheritdoc />
        public IEnumerable<SubscribedHandler> GetSubscribedHandlers(
            SourceModel sourceModel,
            Action<object> subscriptionAction
        )
        {
            var sourceType = sourceModel.ClrType;

            if (sourceType.IsAbstract)
                sourceType = CreateClassProxyType(sourceType);

            var mockSource = FormatterServices.GetUninitializedObject(sourceType);
            subscriptionAction.Invoke(mockSource);

            foreach (var fieldInfo in sourceModel.ClrTypeEventFieldInfos)
            {
                var eventsHandler = (Delegate) fieldInfo.GetValue(mockSource);

                if (eventsHandler != null)
                    yield return new SubscribedHandler(fieldInfo.Name, eventsHandler);
            }
        }
    }
}