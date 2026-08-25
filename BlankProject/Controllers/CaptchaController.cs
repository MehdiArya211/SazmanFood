using Captcha.Net;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Food.Controllers
{
    /// <summary>
    /// کد امنیتی Captcha
    /// </summary>
    public class CaptchaController : Controller
    {
        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            //generate new question
            int captcha = rand.Next(10000, 99999);

            //store answer
            HttpContext.Session.Remove("Captcha");
            HttpContext.Session.SetString("Captcha", captcha.ToString());

            //image stream
            FileContentResult img = null;

            var captchaGenerator = new CaptchaGenerator();
            var result = captchaGenerator.GenerateCaptchaImage(200, 100, captcha.ToString());

            //using (var mem = new MemoryStream())
            //using (var bmp = new Bitmap(70, 30))
            //using (var gfx = Graphics.FromImage((Image)bmp))
            //{
            //    gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //    gfx.SmoothingMode = SmoothingMode.AntiAlias;
            //    gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

            //    //add noise
            //    if (noisy)
            //    {
            //        int i, r, x, y;
            //        var pen = new Pen(Color.Yellow);
            //        for (i = 1; i < 10; i++)
            //        {
            //            pen.Color = Color.FromArgb(
            //            (rand.Next(0, 255)),
            //            (rand.Next(0, 255)),
            //            (rand.Next(0, 255)));

            //            r = rand.Next(0, (130 / 3));
            //            x = rand.Next(0, 130);
            //            y = rand.Next(0, 30);

            //            gfx.DrawEllipse(pen, x - r, y - r, r, r);
            //        }
            //    }

            //    //add question
            //    gfx.DrawString(captcha.ToString(), new Font("Tahoma", 15), Brushes.Gray, 2, 3);

            //    //render as Jpeg
            //    bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    img = this.File(mem.GetBuffer(), "image/Jpeg");
            //}

            img = this.File(result.CaptchaByteData, "image/Jpeg");
            return img;
        }
    }
}
