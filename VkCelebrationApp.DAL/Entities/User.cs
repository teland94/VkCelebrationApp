using System.Collections.Generic;

namespace VkCelebrationApp.DAL.Entities
{
    public class User : EntityBase
    {
        public string UserName { get; set; }

        public long VkUserId { get; set; }

        public string AccessToken { get; set; }
        

        public ICollection<UserCongratulation> UserCongratulations { get; set; }
    }
}
