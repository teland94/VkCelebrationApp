using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface ICongratulationTemplatesService
    {
        IEnumerable<CongratulationTemplateDto> Find(string text, int? maxItems = 5);
    }
}
