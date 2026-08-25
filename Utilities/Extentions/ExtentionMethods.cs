using MD.PersianDateTime;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Extentions
{
    public static class ExtentionMethods
    {
        public static string GetUrlFriendly(this string word)
        {
            if (word == null) return null;
            return word.Trim().Replace("  ", " ")
                        .Replace(" ", "-")
                        .Replace("_", "-")
                        .Replace("%", "-")
                        .Replace(":", "-")
                        .Replace("?", "")
                        .Replace(";", "-")
                        .Replace("*", "-")
                        .Replace("=", "-")
                        .Replace("^", "-")
                        .Replace("#", "-")
                        .Replace("/", "-")
                        .Replace(".", "-")
                        .Replace("\"", "-")
                        .Replace("\'", "-")
                        .Replace("---", "-")
                        .Replace("--", "-");
        }

        public static string GetImgUrlFriendly(this string word)
        {
            if (word == null) return null;
            return word.Replace("  ", " ")
                        .Replace(" ", "-")
                        .Replace("_", "-")
                        .Replace("%", "-")
                        .Replace(":", "-")
                        .Replace("?", "")
                        .Replace(";", "-")
                        .Replace("*", "-")
                        .Replace("=", "-")
                        .Replace("^", "-")
                        .Replace("#", "-")
                        .Replace("/", "-")
                        .Replace(".", "-")
                        .Replace("\"", "-")
                        .Replace("\'", "-")
                        .Replace("---", "-")
                        .Replace("--", "-");
        }


        /// <summary>
        /// ذخیره فایل در یک آدرس مشخص
        /// </summary>
        /// <param name="file">فایل</param>
        /// <param name="SavePath">
        /// مسیر فایل درون پوشه روت. مثلا:
        /// /Uploads/Admin/
        /// </param>
        /// <param name="FileName">نام به همراه اکستنشن</param>
        /// <param name="type">تایپ فایل</param>
        /// <param name="ProjectPhysicalPath">آدرس فیزیکی روت پروژه (بیرون wwwroot(</param>
        /// <returns>نام نهایی فایل ذخیره شده</returns>
        public static async Task<UtilityBaseResult> SaveFile(this IFormFile file, string SavePath, string FileName = null, FileFormat? type = null, string ProjectPhysicalPath = null)
        {
            if (file == null)
                return new UtilityBaseResult(false, "فایل موجود نیست!");

            if (SavePath == null)
                return new UtilityBaseResult(false, "مسیر ذخیره فایل موجود نیست!");

            var fileType = GetFormat(file.FileName);
            if(fileType == FileFormat.Other)
                return new UtilityBaseResult(false, "فقط بارگزاری فایلهای pdf, word, apk, excel و عکس مجاز می باشد.");

            if (!file.IsValidFile(type))
                return new UtilityBaseResult(false, "فرمت فایل بارگذاری شده صحیح نیست!");

            if (FileName == null)
            {
                var ext = file.GetExtension();
                FileName = Guid.NewGuid().ToString() + ext;
            }

            var myPath = ProjectPhysicalPath ?? Directory.GetCurrentDirectory();

            string _Path = Path.Combine(myPath, "wwwroot" + SavePath + FileName);

            var i = 1;
            string BaseName = FileName;
            while (File.Exists(_Path))
            {
                FileName = i + "-" + BaseName;
                _Path = Path.Combine(myPath, "wwwroot" + SavePath + FileName);
                i++;
            }

            using (var stream = new FileStream(_Path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new UtilityBaseResult(true, "ذخیره فایل با موفقیت انجام شد.", FileName);
        }




        #region گرفتن فرمت فایل آپلود شده
        public static FileFormat GetFormat(string FileName)
        {
            var splited = FileName.Split('.');
            if (splited.Length > 0)
            {
                string extansion = "." + splited[splited.Length - 1].ToLower();
                if (extansion.IsImage())
                    return FileFormat.Image;
                if (extansion.IsPdf())
                    return FileFormat.Pdf;
                if (extansion.IsWord())
                    return FileFormat.Docx;
                if (extansion.IsExcel())
                    return FileFormat.Excel;
                if (extansion.IsApk())
                    return FileFormat.Apk;
            }
            return FileFormat.Other;
        }
        #endregion



        /// <summary>
        /// ذخیره عکس
        /// </summary>
        /// <param name="Pic">فایل عکس</param>
        /// <param name="ThumbPath">آدرس عکس کوچک</param>
        /// <param name="LargePath">آدرس عکس بزرگ</param>
        /// <param name="ThumbWidth">اندازه عرض کوچک</param>
        /// <param name="ThumbHeight">اندازه ارتفاع کوچک</param>
        /// <param name="LargeWidth">اندازه عرض بزرگ</param>
        /// <param name="LargeHeight">اندازه ارتفاع بزرگ</param>
        /// <param name="FileName">نام فایل به همراه پسوند</param>
        /// <returns></returns>
        public static async Task<UtilityBaseResult> SavePicture(this IFormFile Pic, string ThumbPath = null, string LargePath = null, int? ThumbWidth = null, int? ThumbHeight = null, int? LargeWidth = null, int? LargeHeight = null, string FileName = null)
        {
            try
            {
                if (Pic == null)
                    return new UtilityBaseResult(false, "عکس موجود نیست!");

                if (!Pic.IsImage())
                    return new UtilityBaseResult(false, "فرمت فایل بارگذاری شده صحیح نیست!");

                if (ThumbPath == null && LargePath == null)
                    return new UtilityBaseResult(false, "آدرس ذخیره عکس وارد نشده است!");

                if (FileName == null)
                {
                    var ext = Pic.GetExtension();
                    FileName = Guid.NewGuid().ToString() + ext;
                }

                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + (ThumbPath ?? LargePath) + FileName);

                var i = 1;
                string BaseName = FileName;
                while (File.Exists(_Path))
                {
                    FileName = i + "-" + BaseName;
                    _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + ThumbPath + FileName);
                    i++;
                }


                // اگر عکس با اندازه اصلی باید ذخیره شود
                if (ThumbWidth == null && ThumbHeight == null && LargeWidth == null && LargeHeight == null)
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        await Pic.CopyToAsync(stream);
                    }

                // برای ذخیره فایل در اندازه thumb
                if (ThumbPath != null)
                {
                    string _ThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + ThumbPath + FileName);
                    using (var stream = new FileStream(_ThumbPath, FileMode.Create))
                    {
                        await Pic.CopyToAsync(stream);
                    }

                    if (ThumbWidth != null && ThumbHeight != null)
                        ImageResizer.CropImage(_ThumbPath, _ThumbPath, ThumbWidth.Value, ThumbHeight.Value, 100);
                }

                // برای ذخیره فایل در اندازه large
                if (LargePath != null)
                {
                    string _LargePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + LargePath + FileName);
                    using (var stream = new FileStream(_LargePath, FileMode.Create))
                    {
                        await Pic.CopyToAsync(stream);
                    }

                    if (LargeWidth != null && LargeHeight != null)
                        ImageResizer.CropImage(_LargePath, _LargePath, LargeWidth.Value, LargeHeight.Value, 100);
                }

                return new UtilityBaseResult(true, "ذخیره عکس با موفقیت انجام شد.", FileName);
            }
            catch (Exception ex)
            {
                return new UtilityBaseResult(false, "ذخیره عکس با خطا همراه بوده است!", ex.Message);
            }

        }


        /// <summary>
        /// حذف عکس
        /// </summary>
        /// <param name="FileName">نام فایل</param>
        /// <param name="ThumbPath">آدرس عکس کوچک</param>
        /// <param name="LargePath">آدرس عکس بزرگ</param>
        /// <returns></returns>
        public static bool DeletePicOrFile(this string FileName, string ThumbPath = null, string LargePath = null)
        {
            try
            {
                if (ThumbPath == null && LargePath == null) return false;

                if (ThumbPath != null)
                {
                    string _ThumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + ThumbPath + FileName);
                    if (File.Exists(_ThumbPath))
                    {
                        File.Delete(_ThumbPath);
                    }
                }

                if (LargePath != null)
                {
                    string _LargePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + LargePath + FileName);
                    if (File.Exists(_LargePath))
                    {
                        File.Delete(_LargePath);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// گرفتن تمام فرزندان در یک ساختار درختی در قالب یک لیست
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="source"></param>
        /// <param name="recursion"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T, R>(this IEnumerable<T> source, Func<T, R> recursion) where R : IEnumerable<T>
        {
            return source.SelectMany(x => (recursion(x) != null && recursion(x).Any()) ? recursion(x).Flatten(recursion) : null)
                         .Where(x => x != null);
        }



        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items,
        Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
            }
        }




        /// <summary>
        /// تبدیل متن با html  به متن خالی
        /// </summary>
        /// <param name="HtmlInput">متن با html</param>
        /// <returns></returns>
        public static string StripHTML(this string HtmlInput)
        {
            return Regex.Replace(HtmlInput, "<.*?>", String.Empty);
        }




        /// <summary>
        /// تبدیل اعداد با کاراکتر های فارسی و عربی به کاراکتر های انگلیسی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToEnglishNumber(this string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string[] arabic = new string[10] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            string[] english = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            for (int i = 0; i < 10; i++)
            {
                input = input.Replace(persian[i], english[i]);
            }

            for (int i = 0; i < 10; i++)
            {
                input = input.Replace(arabic[i], english[i]);
            }
            return input;
        }


        public static string EncodePasswordMd5(this string pass) //Encrypt using MD5   
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)   
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string   
            return BitConverter.ToString(encodedBytes);
        }



        /// <summary>
        /// تبدیل کاراکتر های فارسی و عربی به کاراکتر های فارسی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPersianCharacter(this string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            input = input.Replace('ي', 'ی');
            input = input.Replace('ك', 'ک');

            return input;
        }



        /// <summary>
        /// گرفتن خلاصه از متن با تعداد کاراکتر های دلخواه
        /// </summary>
        /// <returns></returns>
        public static string GetSummary(this string Text, int Count)
        {
            if (string.IsNullOrEmpty(Text)) return "";
            if (Text.Length >= Count)
                return Text.Substring(0, Count) + " [...]";
            else
                return Text;
        }




        /// <summary>
        /// محاشبه سن با تاریخ تولد
        /// </summary>
        /// <param name="BirthDay"></param>
        /// <returns></returns>
        public static string GetFullAge(this DateTime BirthDay)
        {
            var today = DateTime.Today;
            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (BirthDay.Year * 100 + BirthDay.Month) * 100 + BirthDay.Day;

            var year = (a - b) / 10000;
            var month = ((a - b) % 10000) / 100;
            var day = (a - b) % 100;

            string age = year + " سال و " + month + " ماه و " + day + " روز ";
            return age;
        }



        ///// <summary>
        ///// تولید آدرس با استفاده از عنوان در آدرس
        ///// </summary>
        ///// <param name="UrlFriendlyTitle">عنوان مربوط به آبجکت در آدرس</param>
        ///// <returns></returns>
        //public static string GetUrl(this string UrlFriendlyTitle)
        //{
        //    return "/" + Config.UrlPrefix + "/" + UrlFriendlyTitle;
        //}


        #region تبدیل به فرمت قیمت

        #region تبدیل به فرمت قیمت به ریال

        /// <summary>
        /// تبدیل به فرمت قیمت
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string GetPriceFormat(this string price)
        {
            string InPrice = price;
            string OutPrice = "";

            while (InPrice.Length > 3)
            {
                OutPrice = InPrice.Substring(InPrice.Length - 3) + "," + OutPrice;
                InPrice = InPrice.Substring(0, InPrice.Length - 3);
            }
            OutPrice = InPrice + "," + OutPrice;
            return OutPrice.Substring(0, OutPrice.Length - 1);
        }


        public static string GetPriceFormat(this int price)
        {
            return price.ToString("N0");
        }
        public static string? GetPriceFormat(this int? price)
        {
            if (price == null)
                return null;
            return ((int)price).ToString("N0");
        }


        public static string? GetPriceFormat(this long? price)
        {
            if (price == null)
                return null;
            return ((long)price).ToString("N0");
        }

        public static string GetPriceFormat(this long price)
        {
            return price.ToString("N0");
        }

        #endregion



        #region تبدیل به فرمت قیمت به تومان

        public static string? GetToomanPriceFormat(this long? price)
        {
            if (price == null)
                return null;
            return ((long)price / 10).ToString("N0");
        }

        public static string GetToomanPriceFormat(this long price)
        {
            return (price / 10).ToString("N0");
        }



        /// <summary>
        /// تبدیل قیمت به فرمت 95,000
        /// </summary>
        /// <param name="Rialprice"></param>
        /// <returns></returns>
        public static string GetToomanPriceFormat(this int Rialprice)
        {
            return (Rialprice / 10).ToString("N0");
        }


        public static string GetToomanPriceFormat(this int? Rialprice)
        {
            if (Rialprice == null)
                return "";

            return ((int)Rialprice / 10).ToString("N0");
        }



        public static string GetToomanPriceFormat(this double Rialprice)
        {
            return (Rialprice / 10).ToString("N0");
        }


        public static string GetToomanPriceFormat(this double? price)
        {
            if (price == null)
                return "";

            return ((double)price).GetToomanPriceFormat();
        }


        public static string? GetToomanPriceFormat(this string price)
        {
            if (string.IsNullOrEmpty(price))
                return null;
            return long.Parse(price.Trim()).GetToomanPriceFormat();
        }
        #endregion

        #endregion




        /// <summary>
        /// گرفتن شماره کارت 
        /// </summary>
        /// <param name="CardId"></param>
        /// <returns></returns>
        public static string GetCardIdFormat(this string CardId)
        {
            //CardId = String.Format("{0:####-####-####-####}", CardId);

            string InCardId = CardId;
            string OutCardId = "";

            while (InCardId.Length > 4)
            {
                OutCardId = InCardId.Substring(InCardId.Length - 4) + "-" + OutCardId;
                InCardId = InCardId.Substring(0, InCardId.Length - 4);
            }
            OutCardId = InCardId + "-" + OutCardId;
            return OutCardId.Substring(0, OutCardId.Length - 1);
        }







        /// <summary>
        /// گرفتن تاریخ در قالب شمسی به صورت
        /// سه شنبه، 5 دی ماه 1396 20:35
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string GetPersianFormat(this DateTime Date)
        {
            PersianDateTime pd = new PersianDateTime(Date);
            return pd.Hour.ToString("D2") + ":" + pd.Minute.ToString("D2") + " - " + pd.GetLongDayOfWeekName + "، " + pd.Day + " " + pd.MonthName + " ماه " + pd.Year;
        }



        /// <summary>
        /// گرفتن تاریخ در قالب شمسی به صورت
        /// 1401/05/12
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string GetPersianDateFormat(this DateTime Date)
        {
            PersianDateTime pd = new PersianDateTime(Date);
            return pd.Year + "/" + pd.Month.ToString("D2") + "/" + pd.Day.ToString("D2");
        }





        /// <summary>
        /// گرفتن تاریخ در قالب ابجکت تاریخ شمسی
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static PersianDateTime ToPersianDateTime(this DateTime Date)
        {
            var pd = new PersianDateTime(Date) { EnglishNumber = true };
            pd.EnglishNumber = true;
            return pd;
        }


        /// <summary>
        /// گرفتن تاریخ در قالب ابجکت تاریخ شمسی
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static PersianDateTime? ToPersianDateTime(this DateTime? Date)
        {
            if (Date == null)
                return null;
            var pd = new PersianDateTime(Date);
            pd.EnglishNumber = true;
            return pd;
        }


        /// <summary>
        ///  تبدیل نوع مهارت به فارسی
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string SkillTypeToPersian(this string TypeSkillType)
        {
            if (TypeSkillType== "Specialize")
            {
                return "تخصصی";
            }
            else
            return "عمومی";
        }



        /// <summary>
        /// آیا یک کلاس مشخص، پروپرتی با نام مشخص دارد؟
        /// typeof(MyClass).HasProperty("propname");
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }



    }


    public class UtilityBaseResult
    {
        /// <summary>
        /// وضعیت عملیات
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// پیغام مناسب بخصوص در هنگام خطا
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// آبجکتی که در خروجی برمیگردد
        /// </summary>
        public string FileName { get; set; }


        public UtilityBaseResult()
        {

        }

        public UtilityBaseResult(bool _status, string _message, string _fileName = null)
        {
            Status = _status;
            Message = _message;
            FileName = _fileName;
        }
    }


    public static class JavascriptScripts
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// اضافه کردن پراپرتی به آبجک داینامیک سی شارپ
        /// </summary>
        /// <param name="expando">آبجکت</param>
        /// <param name="propertyName">نام پراپرتی</param>
        /// <param name="propertyValue">مقدار مورد نظر</param>
        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }

}