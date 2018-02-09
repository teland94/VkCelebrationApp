using System;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetAge(this DateTime birthDate)
        {
            var now = DateTime.Now;

            var age = now.Year - birthDate.Year;
            if (now.AddYears(age) < birthDate) 
                age--;

            return age;
        }
    }
}
