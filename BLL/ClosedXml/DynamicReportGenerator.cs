using ClosedXML.Excel;
using DTO.Base;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace BLL
{
    /// <summary>
    /// گرفتن خروجی اکسل از مدل مربوطه
    /// </summary>
    public class DynamicReportGenerator<T> where T : class
    {

        /// <summary>
        /// لیست پروپرتی های یک مدل بودن پروپرتی های از نوع لیست و کالکشن و اینام
        /// </summary>
        /// <returns></returns>
        public List<PropertyViewModel> GetProperties()
        {
            var Properties = new List<PropertyViewModel>();

            Type myType = typeof(T);
            var props = myType.GetProperties()
                              .Where(p => !typeof(IList<>).IsAssignableFrom(p.PropertyType)
                                       && !typeof(List<>).IsAssignableFrom(p.PropertyType)
                                       && !typeof(IEnumerable<>).IsAssignableFrom(p.PropertyType)
                                       && !typeof(ICollection<>).IsAssignableFrom(p.PropertyType)
                                       && !typeof(Expression).IsAssignableFrom(p.PropertyType));

            foreach (var item in props)
            {
                var temp = new PropertyViewModel
                {
                    TitleEn = item.Name,
                    TitleFa = GetDisplayName(item) ?? item.Name
                };
                Properties.Add(temp);
            }
            return Properties;
            //PropertyInfo IsDeletedPropery = myType.GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            //if (IsDeletedPropery != null && IsDeletedPropery.PropertyType == typeof(bool))
            //    query = query.Where(x => (bool)IsDeletedPropery.GetValue(x, null) == false);

        }




        /// <summary>
        /// آیای پروپرتی خاصی در مدل وجود دارد؟
        /// </summary>
        /// <param name="propertyname">نام پروپرتی</param>
        /// <returns></returns>
        public bool HasProperty(string propertyname)
        {
            Type myType = typeof(T);
            return myType.GetProperties()
                              .Any(p => p.Name.ToLower() == propertyname.ToLower());
        }




        /// <summary>
        /// گرفتن نام نمایشی پروپرتی
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public string GetDisplayName(PropertyInfo property)
        {
            string displayName = null;
            var attr = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            if (attr != null)
                displayName = attr.Name;
            return displayName;
        }




        /// <summary>
        /// تبدیل لیست به دیتا تیبل
        /// </summary>
        /// <param name="items">لیستی از ایتم ها</param>
        /// <returns></returns>
        public DataTable ToDataTable(IEnumerable<T> items, List<string> SelectedProps = null)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                if (SelectedProps != null && SelectedProps.Contains(prop.Name))
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    string coulmnName = GetDisplayName(prop) ?? prop.Name;
                    dataTable.Columns.Add(coulmnName, type);
                }
            }
            foreach (T item in items)
            {
                //var values = new object[Props.Length];
                DataRow dr = dataTable.NewRow();
                for (int i = 0; i < Props.Length; i++)
                {
                    if (SelectedProps != null && SelectedProps.Contains(Props[i].Name))
                    {
                        //inserting property values to datatable rows
                        //values[i] = Props[i].GetValue(item, null);
                        string coulmnName = GetDisplayName(Props[i]) ?? Props[i].Name;
                        dr[coulmnName] = Props[i].GetValue(item, null) ?? DBNull.Value;
                    }
                }
                //dataTable.Rows.Add(values);
                dataTable.Rows.Add(dr);

            }
            //put a breakpoint here and check datatable
            return dataTable;
        }




        /// <summary>
        /// ساخت گزارش
        /// </summary>
        /// <param name="OutputPath">آدرس خروجی که فایل باید ذخیره شود</param>
        /// <param name="model">مدلی که گزارش از آن گرفته میشود</param>
        /// <param name="SelectedProps">پروپرتی هایی که توسط کاربر برای گزارش انتخاب شدند</param>
        /// <returns></returns>
        public bool GenerateReport(string OutputPath, string SheetTitle, IEnumerable<T> model, List<string> SelectedProps = null)
        {
            try
            {
                var dt = ToDataTable(model, SelectedProps);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(dt, SheetTitle);
                    worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.RightToLeft = true; //راست چین کردن اکسل
                    worksheet.Columns().AdjustToContents();  // Adjust column width
                    worksheet.Rows().AdjustToContents();     // Adjust row heights
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + OutputPath);
                    workbook.SaveAs(_Path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// ساخت فایل اکسل برای گزارش بدون ذخیره روی هارد
        /// </summary>
        /// <param name="OutputPath">آدرس خروجی که فایل باید ذخیره شود</param>
        /// <param name="model">مدلی که گزارش از آن گرفته میشود</param>
        /// <param name="SelectedProps">پروپرتی هایی که توسط کاربر برای گزارش انتخاب شدند</param>
        /// <returns></returns>
        public IXLWorkbook GenerateReportWorkBook(string SheetTitle, IEnumerable<T> model, List<string> SelectedProps = null)
        {
            try
            {
                var dt = ToDataTable(model, SelectedProps);
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(dt, SheetTitle);
                worksheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.RightToLeft = true; //راست چین کردن اکسل
                worksheet.Columns().AdjustToContents();  // Adjust column width
                worksheet.Rows().AdjustToContents();     // Adjust row heights
                return workbook;
            }
            catch (Exception e)
            {
                var workbook = new XLWorkbook();
                return workbook;
            }
        }





        /// <summary>
        /// گرفتن خروجی دیتاتیبل از فایل اکسل و تبدیل به لیستی از آبجکتا 
        /// </summary>
        /// <param name="filePath">آدرس فایل اکسل</param>
        /// <param name="ExcelHasHeader">آیا سطر اول فایل اکسل، هدر و سر ستون است؟</param>
        /// <returns></returns>
        public DataTable ImportExceltoDatatable(string filePath, bool ExcelHasHeader = false)
        {
            // Open the Excel file using ClosedXML.
            // Keep in mind the Excel file cannot be open when trying to read it
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Create a new DataTable.
                DataTable dt = new DataTable();

                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Defining type of data column gives proper data table 
                    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                    //Setting column names as Property names
                    string coulmnName = prop.Name;
                    dt.Columns.Add(coulmnName);
                }


                //Loop through the Worksheet rows.
                bool firstRow = true;
                var rows = workSheet.RowsUsed();
                int start = 0;
                int end = 0;
                foreach (IXLRow row in rows)
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        start = row.FirstCellUsed().Address.ColumnNumber;
                        end = row.LastCellUsed().Address.ColumnNumber;
                    }
                    if (!firstRow || !ExcelHasHeader)
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;

                        foreach (IXLCell cell in row.Cells(start, end))
                        {
                            //var prop = Props.Where(x => x.Name == dt.Columns[i].ColumnName);
                            //var type = dt.Columns[i].DataType;

                            //var val = cell.Value?.ToString();
                            //var val = cell.GetFormattedString();

                            string val = null;
                            if (cell.HasFormula)
                                val = cell.CachedValue.ToString();
                            else
                                val = cell.Value.ToString();

                            dt.Rows[dt.Rows.Count - 1][i] = string.IsNullOrEmpty(val) ? null : val;// cell.GetValue<type>();
                            i++;
                        }
                    }
                    firstRow = false;
                }

                return dt;
            }
        }





        /// <summary>
        /// گرفتن خروجی دیتاتیبل از فایل اکسل و تبدیل به لیستی از آبجکتا 
        /// </summary>
        /// <param name="filePath">آدرس فایل اکسل</param>
        /// <param name="ExcelHasHeader">آیا سطر اول فایل اکسل، هدر و سر ستون است؟</param>
        /// <returns></returns>
        public BaseResult ImportExceltoDatatable(IFormFile file, bool ExcelHasHeader = false)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                // Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(stream))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Get all the properties
                    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Defining type of data column gives proper data table 
                        var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                        //Setting column names as Property names
                        string coulmnName = prop.Name;
                        dt.Columns.Add(coulmnName);
                    }

                    if (workSheet.ColumnsUsed().Count() > dt.Columns.Count)
                        return new BaseResult(false, "فایل اکسل باید حاوی تنها " + dt.Columns.Count + " ستون داده باشد!");


                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    var rows = workSheet.RowsUsed();
                    int start = 0;
                    int end = 0;
                    foreach (IXLRow row in rows)
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            start = row.FirstCellUsed().Address.ColumnNumber;
                            end = row.LastCellUsed().Address.ColumnNumber;
                        }
                        if (!firstRow || !ExcelHasHeader)
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;

                            foreach (IXLCell cell in row.Cells(start, end))
                            {
                                //var prop = Props.Where(x => x.Name == dt.Columns[i].ColumnName);
                                //var type = dt.Columns[i].DataType;

                                //var val = cell.Value?.ToString();
                                //var val = cell.GetFormattedString();

                                string val = null;
                                if (cell.HasFormula)
                                    val = cell.CachedValue.ToString();
                                else
                                    val = cell.Value.ToString();

                                dt.Rows[dt.Rows.Count - 1][i] = string.IsNullOrEmpty(val) ? null : val;// cell.GetValue<type>();
                                i++;
                            }
                        }
                        firstRow = false;
                    }

                    return new BaseResult
                    {
                        Status = true,
                        Model = dt
                    };
                }
            }
        }


    }



    public class PropertyViewModel
    {
        /// <summary>
        /// عنوان انگلیسی پروپرتی
        /// </summary>
        public string TitleEn { get; set; }

        /// <summary>
        /// عنوان فارسی پروپرتی
        /// </summary>
        public string TitleFa { get; set; }
    }

}
