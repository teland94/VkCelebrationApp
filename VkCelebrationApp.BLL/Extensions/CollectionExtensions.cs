using System;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random Rnd = new Random();

        public static VkCollection<T> ToVkCollection<T>(this IEnumerable<T> collection, ulong totalCount)
        {
            return new VkCollection<T>(totalCount, collection);
        }

        public static VkCollectionDto<T> ToVkCollectionDto<T>(this IEnumerable<T> collection, ulong totalCount)
        {
            return new VkCollectionDto<T>(totalCount, collection);
        }

        public static T PickRandom<T>(this IList<T> source)
        {
            var r = Rnd.Next(source.Count);
            return source[r];
        }
    }
}
