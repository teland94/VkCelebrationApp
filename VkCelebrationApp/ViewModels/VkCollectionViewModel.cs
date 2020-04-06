using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using VkNet.Utils.JsonConverter;

namespace VkCelebrationApp.ViewModels
{
    [Serializable]
    [JsonConverter(typeof(VkCollectionJsonConverter))]
    public class VkCollectionViewModel<T> : ReadOnlyCollection<T>, IEnumerable<T>
    {
        [JsonProperty("count")]
        public ulong TotalCount { get; private set; }

        public VkCollectionViewModel(ulong totalCount, IEnumerable<T> list) : base(list.ToList())
        {
            TotalCount = totalCount;
        }

        public new T this[int index] => Items[index];

        public new IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    }
}
