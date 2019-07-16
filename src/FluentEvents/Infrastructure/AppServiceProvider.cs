using System;

namespace FluentEvents.Infrastructure
{
    internal class AppServiceProvider : IAppServiceProvider, IScopedAppServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public AppServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
