using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using VkCelebrationApp.BLL.Attributes;
using VkCelebrationApp.BLL.Extensions;

namespace VkCelebrationApp.BLL.Helpers
{
    public static class ExcelHelpers
    {
        internal static XLWorkbook CollectionToWorkbook<T>(IEnumerable<T> collection, string workSheetName)
        {
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add(workSheetName);

            var props = GetExcelProperties(typeof(T)).ToList();

            var list = collection.ToList();

            foreach (var pi in props)
            {
                ExportToExcelAttribute attribute = pi.Value.GetCustomAttribute<ExportToExcelAttribute>();

                var headerCell = worksheet.Cell(1, attribute.Column + 1).SetValue(attribute.Title);
                headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerCell.Style.Font.Bold = true;

                int pictSize = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    T game = list[i];

                    if (Type.GetTypeCode(pi.Value.PropertyType) == TypeCode.Decimal)
                    {
                        worksheet.Cell(i + 2, attribute.Column + 1).SetValue((decimal)game.GetPropValue(pi.Key));
                    }
                    else if (Type.GetTypeCode(pi.Value.PropertyType) == TypeCode.Int32)
                    {
                        worksheet.Cell(i + 2, attribute.Column + 1).SetValue((int)game.GetPropValue(pi.Key));
                    }
                    else if (Type.GetTypeCode(pi.Value.PropertyType) == TypeCode.Int64)
                    {
                        worksheet.Cell(i + 2, attribute.Column + 1).SetValue((long)game.GetPropValue(pi.Key));
                    }
                    else if (Type.GetTypeCode(pi.Value.PropertyType) == TypeCode.DateTime)
                    {
                        worksheet.Cell(i + 2, attribute.Column + 1).SetValue((DateTime)game.GetPropValue(pi.Key));
                    }
                    else if (pi.Value.PropertyType == typeof(Bitmap))
                    {
                        var bitmap = (Bitmap)game.GetPropValue(pi.Key);
                        var scale = (double)attribute.Size / bitmap.Width;
                        var image = worksheet.AddPicture(bitmap)
                            .MoveTo(worksheet.Cell(i + 2, attribute.Column + 1))
                            .Scale(scale);
                        pictSize = image.Width;
                    }
                    else
                    {
                        var propValue = game.GetPropValue(pi.Key) ?? "Unknown";
                        var strValue = propValue.ToString();
                        if (strValue.Contains("|"))
                        {
                            strValue = strValue.Replace("|", "\n");
                            worksheet.Cell(i + 2, attribute.Column + 1).Style.Alignment.SetWrapText();
                        }
                        worksheet.Cell(i + 2, attribute.Column + 1).SetValue(strValue);
                    }

                    if (attribute.Size > 0 && pi.Value.PropertyType == typeof(Bitmap))
                    {
                        worksheet.Row(i + 2).Height = attribute.Size * 0.75;
                    }
                }

                if (attribute.Fit)
                {
                    worksheet.Column(attribute.Column + 1).AdjustToContents();
                }
                if (attribute.Size > 0)
                {
                    if (pi.Value.PropertyType == typeof(Bitmap))
                    {
                        var excelWidth = (pictSize - 12) / 7d + 1;
                        worksheet.Column(attribute.Column + 1).Width = excelWidth;
                    }
                    else
                    {
                        worksheet.Column(attribute.Column + 1).Width = attribute.Size;
                    }
                }
                worksheet.Column(attribute.Column + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            }

            return workbook;
        }

        private static Dictionary<string, PropertyInfo> GetExcelProperties(Type t, string parent = null, Dictionary<string, PropertyInfo> pis = null)
        {
            if (pis == null)
            {
                pis = new Dictionary<string, PropertyInfo>();
            }

            foreach (var prp in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(prp, typeof(ExportToExcelAttribute)))
                {
                    if (string.IsNullOrWhiteSpace(parent))
                    {
                        pis.Add(prp.Name, prp);
                    }
                    else
                    {
                        pis.Add(parent + "." + prp.Name, prp);
                    }
                }
                if (!prp.PropertyType.IsPrimitive && prp.PropertyType != typeof(string) && prp.PropertyType.IsClass)
                {
                    GetExcelProperties(prp.PropertyType, prp.Name, pis);
                }
            }

            return pis;
        }

        public static byte[] GetExportFile<T>(IEnumerable<T> collection, string workSheetName)
        {
            byte[] content;

            XLWorkbook workbook = CollectionToWorkbook(collection, workSheetName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                content = memoryStream.ToArray();
                memoryStream.Close();
            }
            return content;
        }
    }
}