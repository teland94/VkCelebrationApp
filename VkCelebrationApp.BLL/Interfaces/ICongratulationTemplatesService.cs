using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface ICongratulationTemplatesService
    {
        Task<CongratulationTemplateDto> GetAsync(int id);

        Task<IEnumerable<CongratulationTemplateDto>> GetByUserIdAsync(int userId);

        Task CreateAsync(CongratulationTemplateDto item, int userId);

        Task DeleteAsync(int id);

        Task<IEnumerable<CongratulationTemplateDto>> FindAsync(string text, int userId, int? maxItems = 5);

        Task<CongratulationTemplateDto> GetRandomCongratulationTemplateAsync(int userId);

        Task<IEnumerable<CongratulationTemplateDto>> GetRandomCongratulationTemplatesAsync(int userId, int? count = 5);

        Task UpdateAsync(CongratulationTemplateDto item, int userId);
    }
}