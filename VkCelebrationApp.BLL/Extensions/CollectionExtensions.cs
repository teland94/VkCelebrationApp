using System.Collections.Generic;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class CollectionExtensions
    {
        public static VkCollection<T> ToVkCollection<T>(this IEnumerable<T> collection, ulong totalCount)
        {
            return new VkCollection<T>(totalCount, collection);
        }
    }
}
