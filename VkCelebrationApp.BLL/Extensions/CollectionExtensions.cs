using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class CollectionExtensions
    {
        public static VkCollection<T> ToVkCollection<T>(this IEnumerable<T> collection, ulong totalCount)
        {
            return new VkCollection<T>(totalCount, collection);
        }

        public static VkCollectionDto<T> ToVkCollectionDto<T>(this IEnumerable<T> collection, ulong totalCount)
        {
            return new VkCollectionDto<T>(totalCount, collection);
        }
    }
}
