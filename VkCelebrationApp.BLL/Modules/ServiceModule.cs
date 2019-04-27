using Autofac;
using Microsoft.Extensions.DependencyInjection;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.BLL.Services;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums;

namespace VkCelebrationApp.BLL.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => 
            {
                var services = new ServiceCollection().AddAudioBypass();
                return new VkApi(services);
            }).InstancePerLifetimeScope();

            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<VkCelebrationService>().As<IVkCelebrationService>();
            builder.RegisterType<CongratulationTemplatesService>().As<ICongratulationTemplatesService>();
            builder.RegisterType<UserCongratulationsService>().As<IUserCongratulationsService>();
            builder.RegisterType<FaceApiService>().As<IFaceApiService>();
        }
    }
}