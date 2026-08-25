//using System.IO;
//using System.Web;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Drawing.Drawing2D;
//using System.Threading.Tasks;
//using System.Web.Configuration;
//using System;
//using System.Threading;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Http;

///// <summary>
///// Summary description for ETS_files
///// </summary>
///// 
//namespace Eidetazan
//{
//    namespace Files
//    {

//        //DISK SPACE CLASS
//        public class FolderSpace
//        {
//            int totalSpace = int.Parse(WebConfigurationManager.AppSettings["diskSpace"]);
//            float used;
//            string path;

//            public FolderSpace(string Path)
//            {
//                path = Path;
//                used = getFolderSize(path);
//            }

//            //GET SIZE OF FOLDER
//            long getFolderSize(string Path)
//            {

//                long size = 0;

//                string[] files = Directory.GetFiles(Path);
//                string[] folders = Directory.GetDirectories(Path);

//                foreach (string item in files)
//                {
                    
//                    size += new FileInfo(item).Length;
//                }


//                foreach (string item in folders)
//                {
//                    size += getFolderSize(item);
//                }

//                return size;
//            }

//            //BYTES
//            public float SizeInBytes
//            {
//                get { return used; }
//            }

//            //KB
//            public float SizeInKB
//            {
//                get { return used / 1024f; }
//            }

//            //MB
//            public float SizeInMB
//            {
//                get { return used / 1024f / 1024f; }
//            }


//            //USAGE PERCENTAGE
//            public float UsagePercentage
//            {
//                get { return (SizeInMB / totalSpace) * 100; }
//            }

//            //TOTAL SPACE
//            public int Total
//            {
//                get { return totalSpace; }
//            }

//            // FREE SPACE
//            public float FreeSpace
//            {
//                get { return Total - SizeInMB; }
//            }
//        }


//        public static class ETS_files
//        {
//         //   HttpResponse Response = HttpContext.Current.Response;

//            static string[] videoFormats = new string[] { ".mp4", ".mpeg", ".avi", ".3gp", ".flv", ".mkv" };

//            static string[] audioFormats = new string[] { ".mp3", ".wma", ".wav", ".amr", ".ogg", ".acc"};

//            static string[] ForbidenExtentions = new string[] {".exe" , ".asp" , ".php" , ".aspx" , ".php5" , ".php3",
//                                                      ".bat" , ".dll" };

//            static string[] imageFormats = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp" };

//            //check file
//            public static bool isValid(this IFormFile file)
//            {
//                string ext = getExtention(file);
//                bool isForbiden = false;

//                for (int i = 0; i < ForbidenExtentions.Length; i++)
//                {
//                    if (ForbidenExtentions[i] == ext)
//                    {
//                        isForbiden = true;
//                        break;
//                    }
//                }

//                return !isForbiden;
//            }


//            //CHECK IMAGE
//            public static bool isImage(this IFormFile file)
//            {
//                string ext = getExtention(file);
//                bool isImage = false;

//                for (int i = 0; i < imageFormats.Length; i++)
//                {
//                    if (imageFormats[i] == ext)
//                    {
//                        isImage = true;
//                        break;
//                    }
//                }

//                return isImage;
//            }

//            // CHECK VIDEO
//            public static bool isVideo(this IFormFile file)
//            {
//                string ext = getExtention(file);
//                bool isImage = false;

//                for (int i = 0; i < videoFormats.Length; i++)
//                {
//                    if (videoFormats[i] == ext)
//                    {
//                        isImage = true;
//                        break;
//                    }
//                }

//                return isImage;
//            }
//            // CHECK AUDIO
//            public static bool isAudio(this IFormFile file)
//            {
//                string ext = getExtention(file);
//                bool isImage = false;

//                for (int i = 0; i < audioFormats.Length; i++)
//                {
//                    if (videoFormats[i] == ext)
//                    {
//                        isImage = true;
//                        break;
//                    }
//                }

//                return isImage;
//            }

//            // GET FILE TYPE
//            public static string GetFileType(string FileName)
//            {
//                string ext = getExtention(FileName);
//                ext = ext.ToLower();

//                if (ext == "")
//                    return "folder";

//                foreach (string s in imageFormats)
//                    if (s == ext)
//                        return "image";

//                foreach (string s in audioFormats)
//                    if (s == ext)
//                        return "audio";

//                foreach (string s in videoFormats)
//                    if (s == ext)
//                        return "video";

//                foreach (string s in ForbidenExtentions)
//                    if (s == ext)
//                        return "system";
                
//                return "unknown";
//            }




//            //GET EXTENTION Http Posted File
//            public static string getExtention(this IFormFile file)
//            {
//                string Ext = Path.GetExtension(file.FileName);
//                Ext = Ext.ToLower();

//                return Ext;
//            }

//            //GET EXTENTION String File Name
//            public static string getExtention(string file)
//            {
//                string Ext = Path.GetExtension(file);
//                Ext = Ext.ToLower();

//                return Ext;
//            }

//            // DOWNLOAD SPEED LIMITER
//            //public void DownloadFileWithLimitedSpeed(string fileName, string filePath, long downloadSpeed)
//            //{
//            //    if (!File.Exists(filePath))
//            //    {
//            //        throw new Exception("Err: There is no such a file to download.");
//            //    }

//            //    // Get the BinaryReader instance to the file to download.
//            //    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//            //    {
//            //        using (BinaryReader br = new BinaryReader(fs))
//            //        {

//            //            Response.Buffer = false;

//            //            // The file length.
//            //            long fileLength = fs.Length;

//            //            // The minimum size of a package 1024 = 1 Kb.
//            //            int pack = 1024;

//            //            // The original formula is: sleep = 1000 / (downloadspeed / pack)
//            //            // which equals to 1000.0 * pack / downloadSpeed.
//            //            // And here 1000.0 stands for 1000 millisecond = 1 second
//            //            int sleep = (int)Math.Ceiling(1000.0 * pack / downloadSpeed);


//            //            // Set the Header of the current Response.
//            //            Response.AddHeader("Content-Length", fileLength.ToString());
//            //            Response.ContentType = "application/octet-stream";

//            //            string utf8EncodingFileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
//            //            Response.AddHeader("Content-Disposition", "attachment;filename=" + utf8EncodingFileName);

//            //            // The maxCount stands for a total count that the thread sends the file pack.
//            //            int maxCount = (int)Math.Ceiling(Convert.ToDouble(fileLength) / pack);

//            //            for (int i = 0; i < maxCount; i++)
//            //            {
//            //                if (Response.IsClientConnected)
//            //                {
//            //                    Response.BinaryWrite(br.ReadBytes(pack));

//            //                    // Sleep the response thread after it sends a file pack.
//            //                    Thread.Sleep(sleep);
//            //                }
//            //                else
//            //                {
//            //                    break;
//            //                }
//            //            }

//            //        }
//            //    }
//            //}
//        }


//        public class FolderItem
//        {
//            bool isFolder;
//            string path;
//           public string name;

//            public FolderItem(bool isFolder, string path , string name)
//            {
//                this.path = path;
//                this.isFolder = isFolder;
//                this.name = name;
//            }

//            public string icon
//            {
//                get
//                {return ETS_files.GetFileType(name);}
//            }

//            //GET FOLDER ITEMS
//            public static List<FolderItem> GetDirectoryContents(string path)
//            {

//                List<FolderItem> res = new List<FolderItem>();
//                path = HttpContext.Current.Server.MapPath(path);

//                for (int i = 0; i < 2; i++)
//                {

//                    string[] files;

//                    if (i == 0)
//                        files = Directory.GetDirectories(path);
//                    else
//                        files = Directory.GetFiles(path);

//                    foreach (string s in files)
//                    {
//                        FileInfo f = new FileInfo(s);
//                        FolderItem fi = new FolderItem(i==0, path, f.Name);
//                        res.Add(fi);
//                    }
//                }
//                return res;
//            }

         

//        }


//        // IMAGE RESIZER
//        public class ImageResize
//        {
//            private static ImageCodecInfo jpgEncoder;


//            //-------------------------------------------//
//            //              RESIZE IMAGE                 //
//            //-------------------------------------------//

//            public async static Task OnlyResizeImage(string inFile, string outFile,
//               int ResizeMaxWidth, int ResizeMaxHeight, long level)
//            {
//                byte[] buffer;
//                using (Stream stream = new FileStream(inFile, FileMode.Open))
//                {
//                    buffer = new byte[stream.Length];
//                    await Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead,
//                    buffer, 0, buffer.Length, null);
//                }
//                using (MemoryStream memStream = new MemoryStream(buffer))
//                {
//                    using (Image inImage = Image.FromStream(memStream))
//                    {

//                        double width;
//                        double height;
//                        double InRatio = (double)inImage.Width / inImage.Height;
//                        double OutRatio = (double)ResizeMaxWidth / ResizeMaxHeight;

//                        //if (inImage.Height < inImage.Width)
//                        //{
//                        //    width = maxDimension;
//                        //    height = (maxDimension / (double)inImage.Width) * inImage.Height;
//                        //}
//                        //else
//                        //{
//                        //    height = maxDimension;
//                        //    width = (maxDimension / (double)inImage.Height) * inImage.Width;
//                        //}

//                        if (inImage.Width > ResizeMaxWidth || inImage.Height > ResizeMaxHeight)
//                        {
//                            if (inImage.Width > ResizeMaxWidth && inImage.Height > ResizeMaxHeight)
//                            {
//                                if (OutRatio < InRatio)
//                                {
//                                    width = ResizeMaxWidth;
//                                    height = width / InRatio;
//                                }
//                                else
//                                {
//                                    height = ResizeMaxHeight;
//                                    width = height * InRatio;
//                                }
//                            }
//                            else
//                            {
//                                if (inImage.Width > ResizeMaxWidth)
//                                {
//                                    width = ResizeMaxWidth;
//                                    height = width / InRatio;
//                                }
//                                else
//                                {
//                                    height = ResizeMaxHeight;
//                                    width = height * InRatio;
//                                }
//                            }


//                            using (Bitmap bitmap = new Bitmap((int)width, (int)height))
//                            {
//                                using (Graphics graphics = Graphics.FromImage(bitmap))
//                                {

//                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
//                                    graphics.InterpolationMode =
//                                    InterpolationMode.HighQualityBicubic;
//                                    graphics.DrawImage(inImage, 0, 0, bitmap.Width, bitmap.Height);

//                                    if (inImage.RawFormat.Guid == ImageFormat.Jpeg.Guid)
//                                    {
//                                        if (jpgEncoder == null)
//                                        {
//                                            ImageCodecInfo[] ici =
//                                            ImageCodecInfo.GetImageDecoders();
//                                            foreach (ImageCodecInfo info in ici)
//                                            {
//                                                if (info.FormatID == ImageFormat.Jpeg.Guid)
//                                                {
//                                                    jpgEncoder = info;
//                                                    break;
//                                                }
//                                            }
//                                        }
//                                        if (jpgEncoder != null)
//                                        {
//                                            EncoderParameters ep = new EncoderParameters(1);
//                                            ep.Param[0] = new EncoderParameter(Encoder.Quality,
//                                            level);
//                                            bitmap.Save(outFile, jpgEncoder, ep);
//                                        }
//                                        else
//                                            bitmap.Save(outFile, inImage.RawFormat);
//                                    }
//                                    else
//                                    {
//                                        //
//                                        // Fill with white for transparent GIFs
//                                        //
//                                        graphics.DrawImage(inImage, 0, 0, bitmap.Width, bitmap.Height);
//                                        bitmap.Save(outFile, inImage.RawFormat);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }





//            //-------------------------------------------//
//            //              CROP RESIZE                 //
//            //-------------------------------------------//

//            public async static Task<bool> ResizeImage(string inFile, string outFile,
//               int CropWidth, int CropHeight, long level)
//            {
//                byte[] buffer;
//                using (Stream stream = new FileStream(inFile, FileMode.Open))
//                {
//                    buffer = new byte[stream.Length];
//                    await Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead,
//                    buffer, 0, buffer.Length, null);
//                }
//                using (MemoryStream memStream = new MemoryStream(buffer))
//                {
//                    using (Image inImage = Image.FromStream(memStream))
//                    {
//                        int width = CropWidth;
//                        int marginLeft = 0;
//                        int marginTop = 0;
//                        float ac = (float)inImage.Width / (float)inImage.Height;
//                        int height = Convert.ToInt16(width / ac);

//                        if (width > height)
//                        {
//                            height = CropHeight;
//                            width = Convert.ToInt16(height * ac);
//                            marginLeft = (width - CropWidth) / 2;
//                        }
//                        else if (height > width)
//                        {
//                            marginTop = (height - CropHeight) / 2;
//                        }
//                        using (Bitmap bitmap = new Bitmap((int)width, (int)height))
//                        {
//                            using (Graphics graphics = Graphics.FromImage(bitmap))
//                            {
//                                graphics.SmoothingMode = SmoothingMode.HighQuality;
//                                graphics.InterpolationMode =
//                                InterpolationMode.HighQualityBicubic;

//                                Rectangle crop = new Rectangle(marginLeft, marginTop, CropWidth, CropHeight);

//                                Graphics gr = Graphics.FromImage(bitmap);

//                                gr.DrawImage(inImage, 0, 0, width, height);

//                                Bitmap pic2 = new Bitmap(crop.Width, crop.Height);
//                                Graphics gr2 = Graphics.FromImage(pic2);
//                                gr2.SmoothingMode = SmoothingMode.HighQuality;
//                                gr2.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                                gr2.DrawImage(bitmap, new Rectangle(0, 0, crop.Width, crop.Height), crop, GraphicsUnit.Pixel);

//                                if (inImage.RawFormat.Guid == ImageFormat.Jpeg.Guid)
//                                {
//                                    if (jpgEncoder == null)
//                                    {
//                                        ImageCodecInfo[] ici =
//                                        ImageCodecInfo.GetImageDecoders();
//                                        foreach (ImageCodecInfo info in ici)
//                                        {
//                                            if (info.FormatID == ImageFormat.Jpeg.Guid)
//                                            {
//                                                jpgEncoder = info;
//                                                break;
//                                            }
//                                        }
//                                    }
//                                    if (jpgEncoder != null)
//                                    {
//                                        EncoderParameters ep = new EncoderParameters(1);
//                                        ep.Param[0] = new EncoderParameter(Encoder.Quality,
//                                        level);
//                                        pic2.Save(outFile, jpgEncoder, ep);
//                                    }
//                                    else
//                                        pic2.Save(outFile, inImage.RawFormat);
//                                }
//                                else
//                                {
//                                    //
//                                    // Fill with white for transparent GIFs
//                                    //
//                                    graphics.DrawImage(inImage, 0, 0, bitmap.Width, bitmap.Height);
//                                    pic2.Save(outFile, inImage.RawFormat);
//                                }
//                            }
//                        }
//                    }
//                }

//                return true;
//            }
//        }

//    }




//}
