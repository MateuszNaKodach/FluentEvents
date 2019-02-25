using System;

namespace FluentEvents.Infrastructure
{
    internal class AppServiceProvider : IAppServiceProvider
    {
        private readonly IServiceProvider m_ServiceProvider;

        public AppServiceProvider(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) => m_ServiceProvider.GetService(serviceType);
    }
}
