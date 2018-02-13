using System;

namespace VkCelebrationApp.BLL.Dtos
{
    public class UserCongratulationDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CongratulationDate { get; set; }

        public long VkUserId { get; set; }

        public int? UserId { get; set; }
    }
}
