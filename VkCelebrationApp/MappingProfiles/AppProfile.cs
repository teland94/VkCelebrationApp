using System.Collections.Generic;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.MappingProfiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            CreateMap(typeof(VkCollectionDto<>), typeof(VkCollectionViewModel<>))
                .ConvertUsing(typeof(VkCollectionDtoToVkCollectionViewModelConverter<,>));

            CreateMap<VkUserDto, VkUserViewModel>();

            CreateMap<UserCongratulationDto, UserCongratulationViewModel>();

            CreateMap<SearchParamsViewModel, SearchParamsDto>()
                .BeforeMap((s, d) => d.CanWritePrivateMessage = true);

            CreateMap<SearchUserParamsViewModel, SearchParamsDto>();
        }

        #region Nested Classes

        private class VkCollectionDtoToVkCollectionViewModelConverter<TDto, TVm> : ITypeConverter<VkCollectionDto<TDto>, VkCollectionViewModel<TVm>>
        {
            public VkCollectionViewModel<TVm> Convert(VkCollectionDto<TDto> source, VkCollectionViewModel<TVm> destination, ResolutionContext context)
            {
                var usersVms = context.Mapper.Map<IEnumerable<TDto>, IEnumerable<TVm>>(source);

                return new VkCollectionViewModel<TVm>(source.TotalCount, usersVms);
            }
        }

        #endregion
    }
}
