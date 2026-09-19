using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
using DTO.Entities.MaxaRabbitMQ;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Printing;
using QRCoder;
using Utilities.Extentions;

namespace BLL.ReserveManagment
{
    public class FoodReserveManager : Manager<FoodReserve, ApplicationContext>, IFoodReserveManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        protected readonly IFoodReserveDetailManager _foodReserveDetailManager;

        public FoodReserveManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor,
            IFoodReserveDetailManager foodReserveDetailManager) : base(_Context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext.Session;
            _foodReserveDetailManager = foodReserveDetailManager;
            //_publisher = publisher;
        }

        public BaseResult AddToReserveAndReserveDetail(FoodReserveDTO model)
        {
            var now = DateTime.Now;

            // محاسبه شروع و پایان هفته (با فرض شروع هفته از شنبه)
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Saturday)) % 7;
            DateTime weekStartDate = now.AddDays(-1 * diff).Date;
            DateTime weekEndDate = weekStartDate.AddDays(6).Date;

            var foodReserve = new FoodReserve();

            foodReserve.CreatedAt = DateTime.Now;
            foodReserve.Year = DateTime.Now.Year;
            foodReserve.Month = DateTime.Now.Month;
            foodReserve.WeekStartDate = weekStartDate;
            foodReserve.WeekEndDate = weekEndDate;
            foodReserve.UserId = model.UserId;

            var resultFoodReserve = base.Create(foodReserve);

            var resultFoodReserveDetail = _foodReserveDetailManager.CreateFoodReserveDetail(model.Details.ToList(), resultFoodReserve.Model);


            return resultFoodReserveDetail;


        }

        public List<FoodReserveDetail> GetFoodReserveDetail(long userId)
        {
            var today = DateTime.Today;

            // گرفتن رزرو هفته جاری با متد GetDTO
            var reserve = UOW.FoodReserve.GetDTO<FoodReserveDTO>(
                selector: f => new FoodReserveDTO
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    WeekStartDate = f.WeekStartDate,
                    WeekEndDate = f.WeekEndDate
                },
                filter: f => f.UserId == userId
                            && f.WeekStartDate <= today
                            && f.WeekEndDate >= today
            ).FirstOrDefault();

            if (reserve == null)
                return new List<FoodReserveDetail>();

            // گرفتن جزئیات رزرو مربوط به این رزرو
            var reservDetail = UOW.FoodReserveDetail.Get(
                filter: x => x.FoodReserveId == reserve.Id,
                includeExpressions: q => q
                    .Include(x => x.Day)
                    .Include(x => x.Meal)
                    .Include(x => x.MainFood)
                    .Include(x => x.Dessert)
            ).ToList();



            return reservDetail;
        }

        public async Task<List<WeeklyFoodReserveDTO>> GetWeeklyFoodReserve(long userId)
        {
            var today = DateTime.Today;

            // گرفتن رزرو هفته جاری با متد GetDTO
            var reserve = UOW.FoodReserve.GetDTO<FoodReserveDTO>(
                selector: f => new FoodReserveDTO
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    WeekStartDate = f.WeekStartDate,
                    WeekEndDate = f.WeekEndDate
                },
                filter: f => f.UserId == userId
                            && f.WeekStartDate <= today
                            && f.WeekEndDate >= today
            ).FirstOrDefault();

            if (reserve == null)
                return new List<WeeklyFoodReserveDTO>();

            // گرفتن جزئیات رزرو مربوط به این رزرو
            var reservDetail = UOW.FoodReserveDetail.Get(
                filter: x => x.FoodReserveId == reserve.Id,
                includeExpressions: q => q
                    .Include(x => x.Day)
                    .Include(x => x.Meal)
                    .Include(x => x.MainFood)
                    .Include(x => x.Dessert)
            ).ToList();

            // ساخت لیست خروجی بر اساس جزئیات
            var list = reservDetail.Select(d => new WeeklyFoodReserveDTO
            {
                Date = reserve.WeekStartDate.AddDays(d.Day.Code - 1),
                DayTitle = d.Day?.Title ?? "-",
                FoodReserveDetaileId = d.Id,
                MealTitle = d.Meal?.Title ?? "-",
                MainFood = d.MainFood?.Title ?? "-",
                Dessert = d.Dessert?.Title ?? "-",
                SideDish = d.SideDishId?.ToString() ?? "-"
            }).ToList();

            return list;
        }

        /// <summary>
        /// چاپ ژتون
        /// </summary>
        /// <param name="foodReserveDetaileIds"></param>
        /// <param name="UserId"></param>
        /// <param name="FullName"></param>
        /// <returns></returns>
        public async Task PrintFoodReserveAsyncMaxa(List<long> foodReserveDetaileIds, long UserId, string FullName)
        {

            foreach (var foodReserveDetaileId in foodReserveDetaileIds)
            {

                var detail = UOW.FoodReserveDetail.Get(
                    filter: x => x.Id == foodReserveDetaileId,
                    includeExpressions: q => q
                        .Include(x => x.Day)
                        .Include(x => x.Meal)
                        .Include(x => x.MainFood)
                        .Include(x => x.Dessert)
                        .Include(x => x.SideDish)
                ).FirstOrDefault();



                var foodPrint = new Dima_Printer()
                {
                    Id = Guid.NewGuid(),
                    CorrelationId = Guid.NewGuid(),
                    Employee_Id = UserId,
                    Full_Name = FullName,
                    Meal_Type = detail.Meal?.Title ?? "-",
                    Meal_Details = $"{detail.Day?.Title ?? "-"} | {detail.MainFood?.Title ?? "-"} | {detail.Dessert?.Title ?? "-"} | {detail.SideDish?.Title ?? "-"}",
                    TimeStamp = DateTime.Now
                };



                // await _publisher.Publish(foodPrint);
            }



        }



        private static readonly ConcurrentDictionary<long, byte> _printedFoodReserveDetailIds = new();

        public async Task PrintFoodReserveAsync(
            List<long> foodReserveDetaileIds,
            long userId,
            string fullName
        )
        {
            foreach (var foodReserveDetaileId in foodReserveDetaileIds)
            {
                // جلوگیری از چاپ تکراری/همزمان
                if (!_printedFoodReserveDetailIds.TryAdd(foodReserveDetaileId, 0))
                    continue;

                try
                {
                    var detail = UOW.FoodReserveDetail.Get(
                        filter: x => x.Id == foodReserveDetaileId,
                        includeExpressions: q => q
                            .Include(x => x.Day)
                            .Include(x => x.Meal)
                            .Include(x => x.MainFood)
                            .Include(x => x.Dessert)
                            .Include(x => x.SideDish)
                    ).FirstOrDefault();

                    if (detail == null)
                    {
                        _printedFoodReserveDetailIds.TryRemove(foodReserveDetaileId, out _);
                        continue;
                    }

                    var reserve = UOW.FoodReserve.FirstOrDefault(x =>
                        x.Id == detail.FoodReserveId &&
                        x.UserId == userId);

                    var user = UOW.Users.FirstOrDefault(x =>
                        x.Id == userId &&
                        x.IsDeleted == false &&
                        x.IsEnabled);

                    if (reserve == null || user == null || detail.Day == null)
                    {
                        _printedFoodReserveDetailIds.TryRemove(foodReserveDetaileId, out _);
                        continue;
                    }

                    var personalCode =
                        user.PersonCode?.ToString() ??
                        user.Username;

                    var reserveDate = reserve.WeekStartDate
                        .AddDays(detail.Day.Code - 1)
                        .Date;

                    var nextDate = reserveDate.AddDays(1);

                    var quotaToken = UOW.QoutaPerson
                        .Get(
                            x => x.PersonalCode == personalCode &&
                                 x.IsDeleted == false &&
                                 x.QoutaAllocation != null &&
                                 x.QoutaAllocation.IsDeleted == false &&
                                 x.QoutaAllocation.QoutaAllocationDate >= reserveDate &&
                                 x.QoutaAllocation.QoutaAllocationDate < nextDate &&
                                 x.QoutaAllocation.MealId == detail.MealId,
                            null,
                            null,
                            null,
                            x => x.Include(i => i.QoutaAllocation))
                        .SingleOrDefault();

                    if (quotaToken == null)
                    {
                        _printedFoodReserveDetailIds.TryRemove(foodReserveDetaileId, out _);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(quotaToken.DeliveryCode))
                    {
                        string deliveryCode;
                        do
                        {
                            deliveryCode = Random.Shared.Next(100000, 1000000).ToString();
                        }
                        while (UOW.QoutaPerson.Any(x =>
                            x.DeliveryCode == deliveryCode &&
                            x.IsDeleted == false));

                        quotaToken.DeliveryCode = deliveryCode;
                    }

                    if (string.IsNullOrWhiteSpace(quotaToken.DeliveryHash))
                    {
                        quotaToken.DeliveryHash = Guid.NewGuid().ToString("N");
                    }

                    UOW.QoutaPerson.Update(quotaToken);
                    if (!UOW.Commit())
                        throw new InvalidOperationException("ثبت اطلاعات امنیتی QR ژتون انجام نشد.");

                    var request = httpContextAccessor.HttpContext?.Request;
                    if (request == null || !request.Host.HasValue)
                        throw new InvalidOperationException("آدرس سامانه برای تولید QR در دسترس نیست.");

                    var scanUrl =
                        $"{request.Scheme}://{request.Host}{request.PathBase}" +
                        "/ReserveManagment/TokenScan/Use" +
                        $"?code={Uri.EscapeDataString(quotaToken.DeliveryCode)}" +
                        $"&key={Uri.EscapeDataString(quotaToken.DeliveryHash)}" +
                        $"&mealId={detail.MealId}";

                    using var qrGenerator = new QRCodeGenerator();
                    using var qrData = qrGenerator.CreateQrCode(
                        scanUrl,
                        QRCodeGenerator.ECCLevel.Q);
                    using var qrCode = new PngByteQRCode(qrData);

                    var qrBytes = qrCode.GetGraphic(5);
                    using var qrStream = new MemoryStream(qrBytes);
                    using var qrBitmap = Image.FromStream(qrStream);

                    // متن‌های چاپ
                    string title = "ژتون غذا";
                    string printDate = "تاریخ چاپ: " + DateTime.Now.ToPersianDateTime();
                    string userInfo = "کاربر: " + (fullName ?? "-");
                    string mealType = "وعده: " + (detail.Meal?.Title ?? "-");
                    string mealDetails = $"جزئیات: {detail.Day?.Title ?? "-"} | {detail.MainFood?.Title ?? "-"} | {detail.Dessert?.Title ?? "-"} | {detail.SideDish?.Title ?? "-"}";
                    string tokenNo = "شماره: " + detail.Id;

                    Action<object, PrintPageEventArgs> printerPage = (sender, e) =>
                    {
                        Graphics g = e.Graphics;

                        System.Drawing.Font font = new System.Drawing.Font("Tahoma", 10);
                        System.Drawing.Font boldFont = new System.Drawing.Font("Tahoma", 11, FontStyle.Bold);

                        int pageWidth = e.PageBounds.Width;
                        int margin = 1;
                        int contentWidth = pageWidth - 2 * margin;

                        // راست‌چین فارسی
                        StringFormat rightAlign = new StringFormat(StringFormatFlags.DirectionRightToLeft);

                        var centerAlign = new StringFormat();
                        centerAlign.Alignment = StringAlignment.Center;
                        centerAlign.LineAlignment = StringAlignment.Center;

                        int startY = margin;
                        int offset = 0;

                        // عنوان
                        g.DrawString(title, new System.Drawing.Font("Tahoma", 14, FontStyle.Bold), Brushes.Black,
                            new RectangleF(margin, startY + offset, contentWidth, 35), centerAlign);
                        offset += 35;

                        // خط جداکننده
                        g.DrawLine(Pens.Black, margin, startY + offset, pageWidth - margin, startY + offset);
                        offset += 10;

                        // محتوا
                        g.DrawString(tokenNo, boldFont, Brushes.Black, new RectangleF(margin, startY + offset, contentWidth, 20), rightAlign);
                        offset += 20;

                        g.DrawString(printDate, font, Brushes.Black, new RectangleF(margin, startY + offset, contentWidth, 20), rightAlign);
                        offset += 20;

                        g.DrawString(userInfo, font, Brushes.Black, new RectangleF(margin, startY + offset, contentWidth, 20), rightAlign);
                        offset += 20;

                        g.DrawString(mealType, font, Brushes.Black, new RectangleF(margin, startY + offset, contentWidth, 20), rightAlign);
                        offset += 20;

                        // اگر طولانی شد، دو خطش کن
                        g.DrawString(mealDetails, font, Brushes.Black,
                            new RectangleF(margin, startY + offset, contentWidth, 60), rightAlign);
                        offset += 55;

                        // QR یک‌بارمصرف ژتون
                        const int qrSize = 120;
                        var qrX = margin + Math.Max(0, (contentWidth - qrSize) / 2);
                        g.DrawImage(qrBitmap, new Rectangle(qrX, startY + offset, qrSize, qrSize));
                        offset += qrSize + 4;

                        g.DrawString("برای تحویل غذا اسکن شود", font, Brushes.Black,
                            new RectangleF(margin, startY + offset, contentWidth, 22), centerAlign);
                        offset += 25;

                        // انتها
                        g.DrawLine(Pens.Black, margin, startY + offset, pageWidth - margin, startY + offset);
                        offset += 10;

                        g.DrawString("نوش جان!", boldFont, Brushes.Black,
                            new RectangleF(margin, startY + offset, contentWidth, 25), centerAlign);
                        offset += 25;

                        // کادر
                        g.DrawRectangle(Pens.Black, margin, margin, pageWidth - 2 * margin, offset);
                    };
                    PrintDocument pd = new PrintDocument();
                    pd.PrinterSettings.PrinterName = "RONGTA 58mm Series Printer(2)";
                    pd.PrintPage += (s, e) => printerPage(s, e);
                    pd.Print();
                    // چاپ روی همه پرینترهای انتخاب‌شده

                }
                catch(Exception ex)
                {
                    // اگر خطا خورد، اجازه بده دوباره قابل چاپ بشه
                    _printedFoodReserveDetailIds.TryRemove(foodReserveDetaileId, out _);
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        private static void EnsurePrinterIsValid(string printerName)
        {
            var installed = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

            // اگر اینجا پرینتر نبود => نام اشتباه یا برای این User نصب نیست
            if (!installed.Any(p => string.Equals(p, printerName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"پرینتر '{printerName}' در لیست پرینترهای نصب‌شده برای این کاربر/سرویس وجود ندارد.\n" +
                    $"InstalledPrinters:\n- {string.Join("\n- ", installed)}"
                );

            var ps = new PrinterSettings { PrinterName = printerName };
            if (!ps.IsValid)
                throw new InvalidOperationException($"پرینتر '{printerName}' نصب شده ولی از دید این پروسه معتبر نیست (IsValid=false).");
        }

        // public async Task PrintFoodReserveAsync(long userId, WeeklyFoodReserveDTO meal)
        //public async Task PrintFoodReserveAsync(long userId)
        //{
        //    var foodPrint = new Dima_Printer()
        //    {
        //        Id = Guid.NewGuid(),
        //        CorrelationId = Guid.NewGuid(),
        //        Employee_Id = userId,
        //        Full_Name = "مهدی آریانژاد", // می‌تونی از سشن بگیری
        //        //Full_Name = "کاربر", // می‌تونی از سشن بگیری
        //        Meal_Type = "ناهار",
        //        //Meal_Type = meal.MealTitle,
        //       // Meal_Details = $"{meal.DayTitle} | {meal.MainFood} | {meal.Dessert} | {meal.SideDish}",
        //        Meal_Details = $"سبزی پلو با ماهی",
        //        TimeStamp = DateTime.Now
        //    };

        //    await _publisher.Publish(foodPrint);
        //}
    }

}
