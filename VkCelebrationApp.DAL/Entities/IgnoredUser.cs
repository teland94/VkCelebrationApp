using System;

namespace VkCelebrationApp.DAL.Entities
{
    public class IgnoredUser : EntityBase
    {
        public long VkUserId { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
