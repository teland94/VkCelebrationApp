using System.Collections.Generic;

namespace VkCelebrationApp.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string IdentityId { get; set; }
        public AppUser Identity { get; set; }  // navigation property

        public ICollection<UserCongratulation> UserCongratulations { get; set; }
    }
}