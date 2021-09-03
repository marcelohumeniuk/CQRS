using System;
using CQRS.Infra.CrossCutting.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS.UI.Web.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}