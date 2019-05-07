using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.ViewModels
{
    public class PagedVkCollectionViewModel<T>
    {
        public VkCollectionViewModel<T> VkCollection { get; set; }

        public uint TotalCount { get; set; }
    }
}
