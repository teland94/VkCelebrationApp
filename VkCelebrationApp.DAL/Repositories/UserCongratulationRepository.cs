﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;

namespace VkCelebrationApp.DAL.Repositories
{
    public class UserCongratulationRepository : EfGenericRepository<UserCongratulation>, IUserCongratulationRepository
    {
        public UserCongratulationRepository(DbContext context) 
            : base(context)
        {
        }

        public IEnumerable<long> GetExistsVkIds(IEnumerable<long> vkIds)
        {
            return DbSet.Where(uc => vkIds.Any(vid => vid == uc.VkUserId))
                .Select(uc => uc.VkUserId)
                .ToList();
        }
    }
}
