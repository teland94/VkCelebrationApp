using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.DAL.Configuration;

namespace VkCelebrationApp.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(CreateConnectionStringsConfiguration).SingleInstance();
            builder.Register(CreateFaceApiConfiguration).SingleInstance();
        }

        private static IConnectionStringsConfiguration CreateConnectionStringsConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new ConnectionStringsConfiguration();

            new ConfigureFromConfigurationOptions<ConnectionStringsConfiguration>(configuration.GetSection("ConnectionStrings"))
                .Configure(result);

            return result;
        }

        private static IFaceApiConfiguration CreateFaceApiConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new FaceApiConfiguration();

            new ConfigureFromConfigurationOptions<FaceApiConfiguration>(configuration.GetSection("FaceApi"))
                .Configure(result);

            return result;
        }

        #region Nested Classes

        public class ConnectionStringsConfiguration : IConnectionStringsConfiguration
        {
            public string DefaultConnection { get; set; }
        }

        public class FaceApiConfiguration : IFaceApiConfiguration
        {
            public string Key { get; set; }
            public string Endpoint { get; set; }
        }

        #endregion
    }
}
