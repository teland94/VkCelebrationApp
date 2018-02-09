using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VkCelebrationApp.BLL.Dtos
{
    public class VkCollectionDto<T> : ReadOnlyCollection<T>, IEnumerable<T>
    {
        public ulong TotalCount { get; private set; }

        public VkCollectionDto(ulong totalCount, IEnumerable<T> list) : base(list.ToList())
        {
            TotalCount = totalCount;
        }

        public new T this[int index] => Items[index];

        public new IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    }
}
