using System;

namespace VkCelebrationApp.BLL.Attributes
{
    public class ExportToExcelAttribute : Attribute
    {
        public int Column { get; set; }

        public string Title { get; set; }
        
        public bool Fit { get; set; }

        public int Size { get; set; }

        public ExportToExcelAttribute(int column, string title, bool fit = true, int size = 0)
        {
            Column = column;
            Title = title;
            Fit = fit;
            Size = size;
        }
    }
}
