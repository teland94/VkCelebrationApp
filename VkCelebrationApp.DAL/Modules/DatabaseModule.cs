using Autofac;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Interfaces;

namespace VkCelebrationApp.DAL.Modules
{
    public class DatabaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
        }
    }
}
