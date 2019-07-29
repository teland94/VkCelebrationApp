using System;

namespace VkCelebrationApp.ViewModels
{
    public class UserCongratulationsGetViewModel
    {
        public DateTime? CongratulationDate { get; set; }
        public int? TimezoneOffset { get; set; }
    }

    public class ExportUserCongratulationsGetViewModel
    {
        public int TimezoneOffset { get; set; }
        public DateTime? CongratulationDate { get; set; }
    }
}
