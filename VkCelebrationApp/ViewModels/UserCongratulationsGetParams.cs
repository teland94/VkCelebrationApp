using System;

namespace VkCelebrationApp.ViewModels
{
    public class UserCongratulationsGetParams
    {
        public DateTime? CongratulationDate { get; set; }
        public int? TimezoneOffset { get; set; }
    }
}
