using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace VkCelebrationApp.Autofac
{
    public static class AutofacExtensions
    {
        public static IContainer AddAutofac(this IServiceCollection services, IConfiguration configuration)
        {
            var autofacRegisterer = new AutofacRegisterer();
            var runtime = DependencyContext.Default.Target.Runtime;
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtime);

            autofacRegisterer.RegisterConfiguration(configuration);
            autofacRegisterer.RegisterModules(assemblyNames);

            return autofacRegisterer.Build(services);
        }
    }
}