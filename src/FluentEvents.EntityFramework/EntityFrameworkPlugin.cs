using System;
using FluentEvents.Plugins;
using FluentEvents.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentEvents.EntityFramework
{
    public class EntityFrameworkPlugin : IFluentEventsPlugin
    {
        public void ApplyServices(IServiceCollection services, IServiceProvider appServiceProvider)
        {
            services.AddSingleton<ITypesResolutionService, EntityFrameworkTypesResolutionService>();
        }
    }
}