using System.Collections.Generic;

namespace VkCelebrationApp.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public ICollection<UserCongratulation> UserCongratulations { get; set; }
    }
}
