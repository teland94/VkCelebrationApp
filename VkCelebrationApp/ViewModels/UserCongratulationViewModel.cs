using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VkCelebrationApp.ViewModels
{
    public class UserCongratulationViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public DateTime CongratulationDate { get; set; }

        public VkUserViewModel VkUser { get; set; }
        public long VkUserId { get; set; }
    }
}
