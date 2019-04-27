using System;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.DAL.EF;
using System.Linq;
using VkCelebrationApp.DAL.Extenstions;
using Microsoft.EntityFrameworkCore;

namespace VkCelebrationApp.BLL.Services
{
    internal class CongratulationTemplatesService : ICongratulationTemplatesService
    {
        private ApplicationContext DbContext { get; }

        public CongratulationTemplatesService(ApplicationContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task CreateAsync(CongratulationTemplateDto item, int userId)
        {
            var congratulationTemplate = Mapper.Map<CongratulationTemplateDto, CongratulationTemplate>(item);

            congratulationTemplate.CreatedById = userId;

            await DbContext.CongratulationTemplates.AddAsync(congratulationTemplate);
            await DbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var congratulationTemplate = DbContext.CongratulationTemplates.Find(id);
            DbContext.CongratulationTemplates.Remove(congratulationTemplate);
            await DbContext.SaveChangesAsync();
        }

        public async Task<CongratulationTemplateDto> GetAsync(int id)
        {
            var congratulationTemplate = await DbContext.CongratulationTemplates.FindAsync(id);

            return Mapper.Map<CongratulationTemplate, CongratulationTemplateDto>(congratulationTemplate);
        }

        public async Task<IEnumerable<CongratulationTemplateDto>> GetByUserIdAsync(int userId)
        {
            var congratulationTemplates = DbContext.CongratulationTemplates.Where(u => u.CreatedById == userId);

            return Mapper.Map<IEnumerable<CongratulationTemplate>, 
                IEnumerable<CongratulationTemplateDto>>(await congratulationTemplates.ToListAsync());
        }

        public async Task UpdateAsync(CongratulationTemplateDto item, int userId)
        {
            var congratulationTemplate = Mapper.Map<CongratulationTemplateDto, CongratulationTemplate>(item);

            congratulationTemplate.CreatedById = userId;

            DbContext.CongratulationTemplates.Update(congratulationTemplate);
            await DbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CongratulationTemplateDto>> FindAsync(string text, int userId, int? maxItems = 5)
        {
            var congratulationTemplates = DbContext.CongratulationTemplates.Where(t => t.CreatedById == userId);

            if (!string.IsNullOrWhiteSpace(text))
            {
                congratulationTemplates = congratulationTemplates.Where(t => t.Text.Contains(text, StringComparison.OrdinalIgnoreCase));
            }

            if (maxItems != null)
            {
                congratulationTemplates = congratulationTemplates.Take(maxItems.Value);
            }

            return Mapper.Map<IEnumerable<CongratulationTemplate>, 
                IEnumerable<CongratulationTemplateDto>>(await congratulationTemplates.ToListAsync());
        }

        public async Task<CongratulationTemplateDto> GetRandomCongratulationTemplateAsync(int userId)
        {
            var template = await DbContext.CongratulationTemplates.GetRandomItemAsync(t => t.CreatedById == userId);

            return Mapper.Map<CongratulationTemplate, CongratulationTemplateDto>(template);
        }

        public async Task<IEnumerable<CongratulationTemplateDto>> GetRandomCongratulationTemplatesAsync(int userId, int? count = 5)
        {
            var templates = await DbContext.CongratulationTemplates.GetRandomItemsAsync(count ?? 5, t => t.CreatedById == userId);

            return Mapper.Map<IEnumerable<CongratulationTemplate>, IEnumerable<CongratulationTemplateDto>>(templates);
        }
    }
}