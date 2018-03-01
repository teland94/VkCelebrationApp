﻿using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using VkNet;
using VkNet.Enums.Filters;

namespace VkCelebrationApp.BLL.Services
{
    public class UserCongratulationsService : IUserCongratulationsService
    {
        private IUnitOfWork UnitOfWork { get; }
        private VkApi VkApi { get; }

        public UserCongratulationsService(IUnitOfWork unitOfWork,
            VkApi vkApi)
        {
            UnitOfWork = unitOfWork;
            VkApi = vkApi;
        }

        public VkCollectionDto<VkUserDto> GetNoCongratulatedUsers(VkCollectionDto<VkUserDto> users)
        {
            var existsIds = UnitOfWork.UserCongratulationsRepository.GetExistsVkIds(users.Select(u => u.Id));
            return users.Where(u => existsIds.All(eid => eid != u.Id)).ToVkCollectionDto(users.TotalCount);
        }

        public IEnumerable<UserCongratulationDto> GetUserCongratulations()
        {
            var userCongratulations = UnitOfWork.UserCongratulationsRepository.Get(uc => uc.CongratulationDate, false);

            var userCongratulationDtos = Mapper.Map<IEnumerable<UserCongratulation>, IEnumerable<UserCongratulationDto>>(userCongratulations);

            var ids = userCongratulations.Select(uc => uc.VkUserId);
            var vkUsers = VkApi.Users.Get(ids, 
                ProfileFields.Photo100 | ProfileFields.PhotoMaxOrig)
                .ToDictionary(u => u.Id, u => u);

            foreach(var uc in userCongratulationDtos)
            {
                uc.VkUser = Mapper.Map<VkNet.Model.User, VkUserDto>(vkUsers[uc.VkUserId]);
            }

            return userCongratulationDtos;
        }
    }
}
