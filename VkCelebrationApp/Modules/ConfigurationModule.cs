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
            builder.Register(CreateVkSearchConfiguration).SingleInstance();
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

        private static IVkSearchConfiguration CreateVkSearchConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new VkSearchConfiguration();

            new ConfigureFromConfigurationOptions<VkSearchConfiguration>(configuration.GetSection("VkSearch"))
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

        public class VkSearchConfiguration : IVkSearchConfiguration
        {
            private ushort? _ageFrom;
            private ushort? _ageTo;

            public ushort? AgeFrom
            {
                get => _ageFrom ?? 18;
                set => _ageFrom = value;
            }

            public ushort? AgeTo
            {
                get => _ageTo ?? 30;
                set => _ageTo = value;
            }

            public ushort? Sex { get; set; }

            public long? CityId { get; set; }
        }

        public class FaceApiConfiguration : IFaceApiConfiguration
        {
            public string Key { get; set; }
            public string Endpoint { get; set; }
        }

        #endregion
    }
}
