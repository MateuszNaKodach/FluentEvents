using System;

namespace FluentEvents.ServiceProviders
{
    internal class AppServiceProvider : IRootAppServiceProvider, IScopedAppServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public AppServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
