using System;
using Castle.DynamicProxy;

namespace FluentEvents.Utils
{
    public static class FluentEventsProxyBuilder
    {
        private static readonly IProxyBuilder ProxyBuilder = new DefaultProxyBuilder();
        private static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions();
        private static readonly Type[] EmptyTypes = new Type[0];

        public static Type CreateClassProxyType(Type type) 
            => ProxyBuilder.CreateClassProxyType(type, EmptyTypes, ProxyGenerationOptions);
    }
}
