﻿using System.Threading.Tasks;
using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface ICongratulationTemplateRepository : IGenericRepository<CongratulationTemplate>
    {
        Task<CongratulationTemplate> GetRandomCongratulationTemplateAsync();
    }
}