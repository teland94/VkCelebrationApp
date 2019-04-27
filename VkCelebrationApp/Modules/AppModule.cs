using Autofac;
using VkCelebrationApp.Auth;

namespace VkCelebrationApp.Modules
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtFactory>().As<IJwtFactory>();
        }
    }
}
