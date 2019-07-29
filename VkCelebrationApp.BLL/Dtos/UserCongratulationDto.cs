using System;
using System.Drawing;
using VkCelebrationApp.BLL.Attributes;

namespace VkCelebrationApp.BLL.Dtos
{
    public class UserCongratulationDto
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public DateTime CongratulationDate { get; set; }

        public VkUserDto VkUser { get; set; }
        public long VkUserId { get; set; }
    }

    public class ExportUserCongratulationDto
    {
        [ExportToExcel(0, "ВК ID")]
        public long VkUserId { get; set; }

        [ExportToExcel(1, "Имя", false, Size = 25)]
        public string Name { get; set; }

        [ExportToExcel(2, "Фото", Size = 50)]
        public Bitmap Photo { get; set; }

        [ExportToExcel(3, "Текст", false, Size = 100)]
        public string Text { get; set; }

        [ExportToExcel(4, "Дата")]
        public DateTime CongratulationDate { get; set; }
    }
}
