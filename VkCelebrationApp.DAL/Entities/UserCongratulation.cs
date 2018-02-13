using System;

namespace VkCelebrationApp.DAL.Entities
{
    public class UserCongratulation
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public long VkUserId { get; set; }
        public DateTime CongratulationDate { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}