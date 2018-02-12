using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using VkCelebrationApp.BLL.Utils;

namespace VkCelebrationApp.BLL.Dtos
{
    [Serializable]
    [JsonConverter(typeof(VkCollectionJsonConverter))]
    public class VkCollectionDto<T> : ReadOnlyCollection<T>, IEnumerable<T>
    {
        [JsonProperty("count")]
        public ulong TotalCount { get; private set; }

        public VkCollectionDto(ulong totalCount, IEnumerable<T> list) : base(list.ToList())
        {
            TotalCount = totalCount;
        }

        public new T this[int index] => Items[index];

        public new IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    }
}
