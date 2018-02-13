using System;
using AutoMapper;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using VkCelebrationApp.BLL.Extensions;

namespace VkCelebrationApp.BLL.Services
{
    internal class CongratulationTemplatesService : ICrudService<CongratulationTemplateDto>, ICongratulationTemplatesService
    {
        private IUnitOfWork UnitOfWork { get; }

        public CongratulationTemplatesService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Create(CongratulationTemplateDto item)
        {
            var congratulationTemplate = Mapper.Map<CongratulationTemplateDto, CongratulationTemplate>(item);

            UnitOfWork.CongratulationTemplatesRepository.Create(congratulationTemplate);
        }

        public void Delete(int id)
        {
            UnitOfWork.CongratulationTemplatesRepository.Remove(id);
        }

        public IEnumerable<CongratulationTemplateDto> Get()
        {
            var congratulationTemplates = UnitOfWork.CongratulationTemplatesRepository.Get();

            return Mapper.Map<IEnumerable<CongratulationTemplate>, IEnumerable<CongratulationTemplateDto>>(congratulationTemplates);
        }

        public CongratulationTemplateDto Get(int id)
        {
            var congratulationTemplate = UnitOfWork.CongratulationTemplatesRepository.FindById(id);

            return Mapper.Map<CongratulationTemplate, CongratulationTemplateDto>(congratulationTemplate);
        }

        public void Update(CongratulationTemplateDto item)
        {
            var congratulationTemplate = Mapper.Map<CongratulationTemplateDto, CongratulationTemplate>(item);

            UnitOfWork.CongratulationTemplatesRepository.Update(congratulationTemplate);
        }

        public IEnumerable<CongratulationTemplateDto> Find(string text, int? maxItems = 5)
        {
            IEnumerable<CongratulationTemplate> congratulationTemplates;
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (maxItems != null)
                {
                    congratulationTemplates = UnitOfWork.CongratulationTemplatesRepository
                        .Get(1, maxItems.Value, t => t.Text.Contains(text, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    congratulationTemplates = UnitOfWork.CongratulationTemplatesRepository
                        .Get(t => t.Text.Contains(text, StringComparison.OrdinalIgnoreCase));
                }
            }
            else
            {
                if (maxItems != null)
                {
                    congratulationTemplates = UnitOfWork.CongratulationTemplatesRepository.Get(1, maxItems.Value);
                }
                else
                {
                    congratulationTemplates = UnitOfWork.CongratulationTemplatesRepository.Get();
                }
            }
            return Mapper.Map<IEnumerable<CongratulationTemplate>, IEnumerable<CongratulationTemplateDto>>(congratulationTemplates);
        }
    }
}