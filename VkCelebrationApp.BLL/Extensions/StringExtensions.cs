using System;

namespace VkCelebrationApp.BLL.Extensions
{
    public static class StringExtensions
    {
        public static DateTime? ToFullDateTime(this string strDate)
        {
            if (string.IsNullOrWhiteSpace(strDate)) {
                return null;
            }
            var dateParts = strDate.Split('.');
            if (dateParts.Length < 3) {
                return null;
            }
            return new DateTime(Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]));
        }
    }
}
