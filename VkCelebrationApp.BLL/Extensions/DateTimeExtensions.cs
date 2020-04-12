using System;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetAge(this DateTime birthDate, DateTime? currentDate = null)
        {
            var today = currentDate?.Date ?? DateTime.Today;

            var age = today.Year - birthDate.Year;
            if (today.AddYears(-age) < birthDate) 
                age--;

            return age;
        }
    }
}
