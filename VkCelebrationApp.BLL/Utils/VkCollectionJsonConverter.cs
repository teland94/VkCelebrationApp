using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Utils
{
    public class VkCollectionJsonConverter : JsonConverter
    {
        private static string CountField => "count";

        private string CollectionField { get; }

        public VkCollectionJsonConverter(string collectionField = "items")
        {
            CollectionField = collectionField;
        }

        public VkCollectionJsonConverter()
        {
            CollectionField = "items";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vkCollectionType = value.GetType();

            var t = vkCollectionType.GetGenericArguments()[0];
            var toList = typeof(Enumerable).GetMethod("ToList");
            var constructedToList = toList.MakeGenericMethod(t);
            var castList = constructedToList.Invoke(null, new[] { value });

            var vkCollectionSurrogate = new
            {
                TotalCount = vkCollectionType.GetProperty("TotalCount").GetValue(value),
                Items = castList
            };

            serializer.Serialize(writer, vkCollectionSurrogate);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (!objectType.IsGenericType)
            {
                throw new TypeAccessException();
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var keyType = objectType.GetGenericArguments()[0];

            var constructedListType = typeof(List<>).MakeGenericType(keyType);

            var list = (IList) Activator.CreateInstance(constructedListType);
            
            var vkCollection = typeof(VkCollection<>).MakeGenericType(keyType);
            
            var obj = JObject.Load(reader);
            var response = obj["response"] ?? obj;
            var totalCount = response[CountField].Value<ulong>();
            
            foreach (var item in response[CollectionField])
            {
                list.Add(item.ToObject(keyType));
            }

            return Activator.CreateInstance(vkCollection, totalCount, list);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(VkCollection<>).IsAssignableFrom(objectType);
        }
    }
}
