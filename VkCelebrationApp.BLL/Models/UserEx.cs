using Newtonsoft.Json;
using VkNet.Model;

namespace VkCelebrationApp.BLL.Models
{
    public class UserEx : User
    {
        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }
    }
}
