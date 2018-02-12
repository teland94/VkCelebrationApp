using System.Collections.Generic;
using Autofac;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.BLL.Services;
using VkCelebrationApp.DAL.Entities;
using VkNet;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VkApi>().SingleInstance();
            builder.RegisterType<VkCelebrationService>().As<IVkCelebrationService>();
            builder.RegisterType<CongratulationTemplatesService>().As<ICrudService<CongratulationTemplateDto>>();
            builder.RegisterType<CongratulationTemplatesService>().As<ICongratulationTemplatesService>();

            builder.RegisterType<VkCelebrationTelegramBotService>().As<IVkCelebrationTelegramBotService>().SingleInstance();

            ConfigureMapper();
        }

        private static void ConfigureMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<VkNet.Model.User, UserDto>()
                    .ForMember(
                        d => d.Age,
                        opt => opt.MapFrom(m => ConvertBirthDateToAge(m.BirthDate)));

                cfg.CreateMap(typeof(VkCollection<>), typeof(VkCollectionDto<>))
                    .ConvertUsing(typeof(VkCollectionToVkCollectionDtoConverter<,>));

                cfg.CreateMap<CongratulationTemplate, CongratulationTemplateDto>();
                cfg.CreateMap<CongratulationTemplateDto, CongratulationTemplate>();

                cfg.CreateMap<UserCongratulation, UserCongratulationDto>();
                cfg.CreateMap<UserCongratulationDto, UserCongratulation>();
            });
        }

        private static ushort? ConvertBirthDateToAge(string birthDate)
        {
            var date = birthDate.ToFullDateTime();
            return (ushort?)date?.GetAge();
        }

        #region Nested Classes

        private class VkCollectionToVkCollectionDtoConverter<T, TDto> : ITypeConverter<VkCollection<T>, VkCollectionDto<TDto>>
        {
            public VkCollectionDto<TDto> Convert(VkCollection<T> source, VkCollectionDto<TDto> destination, ResolutionContext context)
            {
                var usersDto = context.Mapper.Map<IEnumerable<T>, IEnumerable<TDto>>(source);

                return new VkCollectionDto<TDto>(source.TotalCount, usersDto);
            }
        }

        #endregion
    }

}
