using Autofac;
using VkCelebrationApp.DAL.EF;

namespace VkCelebrationApp.DAL.Modules
{
    public class DatabaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationContext>();
        }
    }
}
