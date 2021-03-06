﻿using System;
using AutoMapper;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.DAL.Entities;
using VkNet.Model;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.MappingProfiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<VkNet.Model.User, VkUserDto>()
                .ForMember(
                    d => d.Age,
                    opt => opt.MapFrom((s, d, destMember, context) => ConvertBirthDateToAge(s.BirthDate, (DateTime?)context.Items["CurrentDate"])))
                .ForMember(
                    d => d.CityId,
                    opt => opt.MapFrom(m => m.City.Id))
                .ForMember(
                    d => d.CountryId,
                    opt => opt.MapFrom(m => m.Country.Id));

            CreateMap(typeof(VkCollection<>), typeof(VkCollectionDto<>))
                .ConvertUsing(typeof(VkCollectionToVkCollectionDtoConverter<,>));

            CreateMap<DAL.Entities.User, UserDto>();
            CreateMap<UserDto, DAL.Entities.User>();

            CreateMap<CongratulationTemplate, CongratulationTemplateDto>();
            CreateMap<CongratulationTemplateDto, CongratulationTemplate>();

            CreateMap<UserCongratulation, UserCongratulationDto>();
            CreateMap<UserCongratulationDto, UserCongratulation>();

            CreateMap<City, VkCityDto>();
            CreateMap<University, VkUniversityDto>();
        }

        private static ushort? ConvertBirthDateToAge(string birthDate, DateTime? currentDate)
        {
            var date = birthDate.ToFullDateTime();
            return (ushort?)date?.GetAge(currentDate);
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
