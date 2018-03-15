using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface ICongratulationTemplatesService : ICrudService<CongratulationTemplateDto>
    {
        IEnumerable<CongratulationTemplateDto> Find(string text, int? maxItems = 5);

        Task<CongratulationTemplateDto> GetRandomCongratulationTemplateAsync();

        Task<IEnumerable<CongratulationTemplateDto>> GetRandomCongratulationTemplatesAsync(int? count = 5);
    }
}