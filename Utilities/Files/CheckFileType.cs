using Microsoft.AspNetCore.Http;
using Myrmec;

namespace Utilities.Extentions
{

    #region مشخص کردن نوع قایل با توجه به هدر و پسوند فایل
    public static class CheckFileType
    {
        #region فرمت های مجاز برای پسوند فایلهای مختلف
        static string[] videoFormats = new string[] { ".mp4", ".mpeg", ".avi", ".3gp", ".flv", ".mkv" };
        static string[] imageFormats = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp" };
        static string[] audioFormats = new string[] { ".mp3", ".ogg", ".wma", ".wav" };
        static string[] pdfFormats = new string[] { ".pdf" };
        static string[] zipFormats = new string[] { ".zip", ".rar" };
        static string[] wordFormats = new string[] { ".doc", ".docx" };
        static string[] excelFormats = new string[] { ".xls", ".xlsx" };
        static string[] apkFormats = new string[] { ".apk" };
        static string[] powerPointFormats = new string[] { ".pptx", ".ppt" };
        static string[] allFormats = videoFormats.Concat(imageFormats).Concat(audioFormats)
            .Concat(pdfFormats).Concat(zipFormats).Concat(wordFormats).Concat(excelFormats)
            .Concat(powerPointFormats).Concat(apkFormats).ToArray();
        #endregion



        #region هدر های مجاز برای فایل های مختلف
        static List<Record> imageHeaders = new List<Record>
                    {
                        new Record("jpg,jpeg", "FF D8 FF E0 ?? ?? 4A 46 49 46 00 01"),
                        new Record("jpg,jpeg", "FF D8 FF E1 ?? ?? 45 78 69 66 00 00"),
                        new Record("jpg,jpeg", "ff,d8,ff,db"),
                        new Record("png", "89,50,4e,47,0d,0a,1a,0a"),
                        new Record("bmp dib", "42 4D"),
                        new Record("gif", "47 49 46 38 37 61"),
                        new Record("gif", "47 49 46 38 39 61"),
                        new Record("tif tiff", "49 49 2A 00"),
                        new Record("tif tiff", "4D 4D 00 2A"),
                    };

        static List<Record> videoHeaders = new List<Record>
                    {
                        new Record("3gp 3g2", "66 74 79 70 33 67", 4),
                        new Record("mpg mpeg", "00 00 01 BA"),
                        new Record("mpg mpeg", "00 00 01 B3"),
                        new Record("mp4", "66 74 79 70 69 73 6F 6D", 4),
                        new Record("mp4", "66 74 79 70 6D 70 34 32", 4),
                        new Record("mp4", "6D 70 34 32", 4),
                        new Record("avi", "41 56 49 20"),
                        new Record("avi", "52 49 46 46"),
                        new Record("flv", "46 4C 56"),
                        new Record("mkv mka mks mk3d webm", "1A 45 DF A3"),
                    };

        static List<Record> pdfHeaders = new List<Record>
                    {
                        new Record("pdf", "25 50 44 46"),
                        new Record("pdf", "25 50 44 46 2D")
                    };

        static List<Record> officeHeaders = new List<Record>
                    {
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,03,04"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,07,08"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,05,06"),
                        new Record("docx xlsx pptx", "50 4B 03 04 14 00 06 00"),
                        new Record("doc xls ppt msg", "D0 CF 11 E0 A1 B1 1A E1"),
                    };

        static List<Record> zipHeaders = new List<Record>
                    {
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,03,04"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,07,08"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,05,06"),
                        new Record("rar", "52,61,72,21,1a,07,00"),
                        new Record("rar", "52,61,72,21,1a,07,01,00"),
                    };

        static List<Record> apkHeaders = new List<Record>
                    {
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,03,04"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,07,08"),
                        new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,05,06"),
                    };

        static List<Record> audioHeaders = new List<Record>
                    {
                        new Record("mp3", "FF FB"),
                        new Record("mp3", "49 44 33"),
                        new Record("mp3", "FF F3"),
                        new Record("mp3", "FF F2"),
                        new Record("ogg oga ogv", "4F 67 67 53"),
                        new Record("wma wmv asf", "30 26 B2 75 8E 66 CF 11"),
                        new Record("wma wmv asf", "A6 D9 00 AA 00 62 CE 6C"),
                        new Record("asf wma wmv", "30 26 B2 75 8E 66 CF 11 A6 D9 00 AA 00 62 CE 6C"),
                    };

        static List<Record> allHeaders = imageHeaders.Concat(videoHeaders).Concat(pdfHeaders)
            .Concat(officeHeaders).Concat(zipHeaders).Concat(audioHeaders).Concat(apkHeaders).ToList();
        #endregion




        #region خواندن بایت های هدر فایل
        private static byte[] ReadFileHead(IFormFile file)
        {
            using var fs = new BinaryReader(file.OpenReadStream());

            // برای آفست دادن که برای فایلهای ویدیویی استفاده می شود
            //fs.BaseStream.Position = offset;

            var bytes = new byte[20];
            fs.Read(bytes, 0, 20);
            return bytes;
        }
        #endregion



        #region گرفتن پسوند فایل
        public static string GetExtension(this IFormFile file)
        {
            return Path.GetExtension(file.FileName)?.ToLower();
        }
        #endregion



        #region بررسی اینکه فایل در مجموع جزو فرمت های مجاز است یا خیر
        /// <summary>
        /// فایل با توجه به تایپ مشخص شده معتبر است یا خیر؟
        /// <para>
        /// بررسی فایل با بایت ها انجام میشود
        /// </para>
        /// </summary>
        /// <param name="file">فایل</param>
        /// <param name="type">نوع فایل مورد نظر</param>
        /// <returns></returns>
        public static bool IsValidFile(this IFormFile file, FileFormat? type = null)
        {
            var supportedFiles = new List<Record>();
            switch (type)
            {
                case FileFormat.Image:
                    supportedFiles = imageHeaders;
                    break;
                case FileFormat.Video:
                    supportedFiles = videoHeaders;
                    break;
                case FileFormat.Docx:
                case FileFormat.Excel:
                case FileFormat.Powerpoint:
                    supportedFiles = officeHeaders;
                    break;
                case FileFormat.Pdf:
                    supportedFiles = pdfHeaders;
                    break;
                case FileFormat.Zip:
                    supportedFiles = zipHeaders;
                    break;
                case FileFormat.Audio:
                    supportedFiles = audioHeaders;
                    break;
                case FileFormat.Apk:
                    supportedFiles = apkHeaders;
                    break;
                default:
                    supportedFiles = allHeaders;
                    break;
            }

            var sniffer = new Sniffer();
            sniffer.Populate(supportedFiles);
            byte[] fileHead = ReadFileHead(file);
            var results = sniffer.Match(fileHead);
            if (results.Count > 0)
            {
                string ext = file.GetExtension();
                return ext.IsValidFile();
            }
            else
                return false;
        }



        #region آیا فایل جزو پسوند های مجاز است؟
        /// <summary>
        /// آیا فایل جزو پسوند های مجاز است؟
        /// </summary>
        /// <param name="ext">پسوند فایل با نقطه در ابتدا</param>
        /// <returns></returns>
        public static bool IsValidFile(this string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            return allFormats.Any(x => x == ext);
        }
        #endregion 
        #endregion



        #region بررسی تصویر
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک تصویر است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsImage(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;
            return imageFormats.Any(x => x == ThExt);
        }

        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها تصویر است؟
        /// </summary>
        public static bool IsImage(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Image)) return false;
            string ext = file.GetExtension();
            return ext.IsImage();
        }


        #region آیا فایل از نوع گیف است؟
        public static bool IsGif(this string extension) => ".gif".Equals(extension, StringComparison.OrdinalIgnoreCase);
        #endregion
        #endregion



        #region بررسی ویدیو
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک ویدیو است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsVideo(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return videoFormats.Any(x => x == ThExt);
        }


        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها ویدیو است؟
        /// </summary>
        public static bool IsVideo(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Video)) return false;
            string ext = file.GetExtension();
            return ext.IsVideo();
        }
        #endregion



        #region بررسی فایل صوتی
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل صوتی است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsAudio(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return audioFormats.Any(x => x == ThExt);
        }


        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل صوتی است؟
        /// </summary>
        public static bool IsAudio(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Audio)) return false;
            string ext = file.GetExtension();
            return ext.IsAudio();
        }
        #endregion



        #region بررسی زیپ
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل زیپ است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsZip(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return zipFormats.Any(x => x == ThExt);
        }


        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل زیپ است؟
        /// </summary>
        public static bool IsZip(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Zip)) return false;
            string ext = file.GetExtension();
            return ext.IsZip();
        }
        #endregion



        #region بررسی pdf
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل پی دی اف است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsPdf(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return pdfFormats.Any(x => x == ThExt);
        }


        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل پی دی اف است؟
        /// </summary>
        public static bool IsPdf(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Pdf)) return false;
            string ext = file.GetExtension();
            return ext.IsPdf();
        }
        #endregion



        #region بررسی word
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل ورد است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsWord(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return wordFormats.Any(x => x == ThExt);
        }


        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل ورد است؟
        /// </summary>
        public static bool IsWord(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Docx)) return false;
            string ext = file.GetExtension();
            return ext.IsWord();
        }
        #endregion



        #region بررسی اکسل
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل اکسل است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsExcel(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return excelFormats.Any(x => x == ThExt);
        }

        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل اکسل است؟
        /// </summary>
        public static bool IsExcel(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Excel)) return false;
            string ext = file.GetExtension();
            return ext.IsExcel();
        }
        #endregion



        #region بررسی پاورپوینت
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل پاور پوینت است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsPowerPoint(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return powerPointFormats.Any(x => x == ThExt);
        }

        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل پاورپوینت است؟
        /// </summary>
        public static bool IsPowerPoint(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Powerpoint)) return false;
            string ext = file.GetExtension();
            return ext.IsPowerPoint();
        }
        #endregion



        #region بررسی نرم افزار موبایل
        /// <summary>
        /// آیا با توجه به پسوند فایل، یک فایل نرم افزار موبایل است؟
        /// </summary>
        /// <param name="ThExt">پسوند فایل</param>
        /// <returns></returns>
        public static bool IsApk(this string ThExt)
        {
            if (string.IsNullOrEmpty(ThExt)) return false;

            return apkFormats.Any(x => x == ThExt);
        }

        /// <summary>
        /// آیا فایل با توجه به هدر و بایت ها فایل نرم افزار موبایل است؟
        /// </summary>
        public static bool IsApk(this IFormFile file)
        {
            if (file == null) return false;
            if (!IsValidFile(file, FileFormat.Apk)) return false;
            string ext = file.GetExtension();
            return ext.IsApk();
        }
        #endregion



    }
    #endregion
}
