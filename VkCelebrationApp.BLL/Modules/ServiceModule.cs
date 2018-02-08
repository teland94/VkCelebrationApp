using Autofac;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.BLL.Services;
using VkNet;
using VkNet.Model;

namespace VkCelebrationApp.BLL.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VkApi>().SingleInstance();
            builder.RegisterType<VkCelebrationService>().As<IVkCelebrationService>();

            ConfigureMapper();
        }

        private static void ConfigureMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
            });
        }
    }
}
