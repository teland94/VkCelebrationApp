using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VkCelebrationApp.Autofac
{
    internal class AutofacRegisterer
    {
        private readonly ContainerBuilder _builder;

        public AutofacRegisterer()
        {
            _builder = new ContainerBuilder();
        }

        public void RegisterModules(IEnumerable<AssemblyName> assemblyNames)
        {
            var assemblies = assemblyNames
                .Where(name => name.Name.StartsWith("VkCelebration"))
                .Distinct()
                .Select(Assembly.Load);

            _builder.RegisterAssemblyModules(assemblies.ToArray());
        }

        public void RegisterConfiguration(IConfiguration configuration)
        {
            _builder.Register(context => configuration);
        }

        public IContainer Build(IServiceCollection services)
        {
            _builder.Populate(services);
            return _builder.Build();
        }
    }
}
