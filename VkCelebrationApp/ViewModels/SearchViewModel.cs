using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.ViewModels
{
    public class SearchViewModel
    {
        public SearchParamsDto SearchParams { get; set; }

        public uint? PageNumber = 1;

        public uint? PageSize = 20;
    }
}
