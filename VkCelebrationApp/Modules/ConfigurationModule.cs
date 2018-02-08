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
            builder.Register(CreateVkApiConfiguration).SingleInstance();
            builder.Register(CreateBotConfiguration).SingleInstance();
        }

        private static IConnectionStringsConfiguration CreateConnectionStringsConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new ConnectionStringsConfiguration();

            new ConfigureFromConfigurationOptions<ConnectionStringsConfiguration>(configuration.GetSection("ConnectionStrings"))
                .Configure(result);

            return result;
        }

        private static IVkApiConfiguration CreateVkApiConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new VkApiConfiguration();

            new ConfigureFromConfigurationOptions<VkApiConfiguration>(configuration.GetSection("VkApi"))
                .Configure(result);

            return result;
        }

        private static IBotConfiguration CreateBotConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            var result = new BotConfiguration();

            new ConfigureFromConfigurationOptions<BotConfiguration>(configuration.GetSection("Bot"))
                .Configure(result);

            return result;
        }

        #region Nested Class

        public class ConnectionStringsConfiguration : IConnectionStringsConfiguration
        {
            public string DefaultConnection { get; set; }
        }

        public class VkApiConfiguration : IVkApiConfiguration
        {
            public ulong AppId { get; set; }
            public string Host { get; set; }
            public int? Port { get; set; }
        }

        public class BotConfiguration : IBotConfiguration
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public string Key { get; set; }
        }

        #endregion
    }
}
