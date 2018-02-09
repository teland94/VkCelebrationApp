using System.Collections.Generic;
using Autofac;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.BLL.Services;
using VkNet;
using VkNet.Model;
using VkNet.Utils;

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
                cfg.CreateMap<User, UserDto>()
                    .ForMember(
                        d => d.Age,
                        opt => opt.MapFrom(m => ConvertBirthDateToAge(m.BirthDate)));

                cfg.CreateMap(typeof(VkCollection<>), typeof(VkCollectionDto<>))
                    .ConvertUsing(typeof(VkCollectionToVkCollectionDtoConverter<,>));
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
