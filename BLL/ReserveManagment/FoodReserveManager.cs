using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
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


        private static readonly ConcurrentDictionary<long, byte> _printedFoodReserveDetailIds = new();

        public async Task<BaseResult> PrintFoodReserveAsync(
            List<long> foodReserveDetaileIds,
            long userId,
            string fullName
        )
        {
            if (foodReserveDetaileIds == null ||
                !foodReserveDetaileIds.Any())
            {
                return new BaseResult(
                    false,
                    "هیچ ژتونی برای چاپ انتخاب نشده است.");
            }

            const string printerName =
                "RONGTA 58mm Series Printer(2)";

            try
            {
                EnsurePrinterIsValid(printerName);
            }
            catch (Exception ex)
            {
                return new BaseResult(
                    false,
                    ex.Message);
            }

            var printedCount = 0;
            var failedCount = 0;
            var errors =
                new List<string>();

            foreach (var foodReserveDetaileId in
                     foodReserveDetaileIds.Distinct())
            {
                if (!_printedFoodReserveDetailIds.TryAdd(
                        foodReserveDetaileId,
                        0))
                {
                    failedCount++;
                    errors.Add(
                        $"ژتون {foodReserveDetaileId} در حال چاپ است.");
                    continue;
                }

                try
                {
                    var detail =
                        UOW.FoodReserveDetail.Get(
                            filter: x =>
                                x.Id ==
                                foodReserveDetaileId,
                            includeExpressions: q => q
                                .Include(x => x.Day)
                                .Include(x => x.Meal)
                                .Include(x => x.MainFood)
                                .Include(x => x.Dessert)
                                .Include(x => x.SideDish))
                        .FirstOrDefault();

                    if (detail == null)
                    {
                        failedCount++;
                        errors.Add(
                            $"جزئیات ژتون {foodReserveDetaileId} یافت نشد.");
                        continue;
                    }

                    var reserve =
                        UOW.FoodReserve.FirstOrDefault(
                            x =>
                                x.Id ==
                                detail.FoodReserveId &&
                                x.UserId == userId);

                    var user =
                        UOW.Users.FirstOrDefault(
                            x =>
                                x.Id == userId &&
                                x.IsDeleted == false &&
                                x.IsEnabled);

                    if (reserve == null ||
                        user == null ||
                        detail.Day == null)
                    {
                        failedCount++;
                        errors.Add(
                            $"اطلاعات رزرو ژتون {foodReserveDetaileId} کامل نیست.");
                        continue;
                    }

                    /*
                     * سیستم رزرو جدید از UnitQuotaPerson استفاده می‌کند.
                     * برای سازگاری با ژتون‌های قدیمی، اگر QoutaPerson وجود
                     * داشته باشد QR قدیمی نیز چاپ می‌شود؛ اما نبود آن دیگر
                     * مانع چاپ فیزیکی ژتون نیست.
                     */
                    Bitmap qrBitmap = null;

                    try
                    {
                        var personalCode =
                            user.PersonCode?
                                .ToString() ??
                            user.Username;

                        var reserveDate =
                            reserve.WeekStartDate
                                .AddDays(
                                    detail.Day.Code - 1)
                                .Date;

                        var nextDate =
                            reserveDate.AddDays(1);

                        var quotaToken =
                            UOW.QoutaPerson
                                .Get(
                                    x =>
                                        x.PersonalCode ==
                                            personalCode &&
                                        x.IsDeleted == false &&
                                        x.QoutaAllocation != null &&
                                        x.QoutaAllocation
                                            .IsDeleted == false &&
                                        x.QoutaAllocation
                                            .QoutaAllocationDate >=
                                            reserveDate &&
                                        x.QoutaAllocation
                                            .QoutaAllocationDate <
                                            nextDate &&
                                        x.QoutaAllocation
                                            .MealId ==
                                            detail.MealId,
                                    null,
                                    null,
                                    null,
                                    x => x.Include(
                                        i =>
                                            i.QoutaAllocation))
                                .SingleOrDefault();

                        if (quotaToken != null)
                        {
                            if (string.IsNullOrWhiteSpace(
                                    quotaToken.DeliveryCode))
                            {
                                string deliveryCode;

                                do
                                {
                                    deliveryCode =
                                        Random.Shared
                                            .Next(
                                                100000,
                                                1000000)
                                            .ToString();
                                }
                                while (
                                    UOW.QoutaPerson.Any(
                                        x =>
                                            x.DeliveryCode ==
                                                deliveryCode &&
                                            x.IsDeleted ==
                                                false));

                                quotaToken.DeliveryCode =
                                    deliveryCode;
                            }

                            if (string.IsNullOrWhiteSpace(
                                    quotaToken.DeliveryHash))
                            {
                                quotaToken.DeliveryHash =
                                    Guid.NewGuid()
                                        .ToString("N");
                            }

                            UOW.QoutaPerson.Update(
                                quotaToken);

                            if (!UOW.Commit())
                            {
                                throw new
                                    InvalidOperationException(
                                        "ثبت اطلاعات امنیتی QR ژتون انجام نشد.");
                            }

                            var request =
                                httpContextAccessor
                                    .HttpContext?
                                    .Request;

                            if (request != null &&
                                request.Host.HasValue)
                            {
                                var scanUrl =
                                    $"{request.Scheme}://{request.Host}{request.PathBase}" +
                                    "/ReserveManagment/TokenScan/Use" +
                                    $"?code={Uri.EscapeDataString(quotaToken.DeliveryCode)}" +
                                    $"&key={Uri.EscapeDataString(quotaToken.DeliveryHash)}" +
                                    $"&mealId={detail.MealId}";

                                using var qrGenerator =
                                    new QRCodeGenerator();

                                using var qrData =
                                    qrGenerator.CreateQrCode(
                                        scanUrl,
                                        QRCodeGenerator
                                            .ECCLevel.Q);

                                using var qrCode =
                                    new PngByteQRCode(
                                        qrData);

                                var qrBytes =
                                    qrCode.GetGraphic(5);

                                using var qrStream =
                                    new MemoryStream(
                                        qrBytes);

                                using var image =
                                    Image.FromStream(
                                        qrStream);

                                qrBitmap =
                                    new Bitmap(image);
                            }
                        }
                    }
                    catch
                    {
                        /*
                         * خطای QR نباید چاپ فیزیکی ژتون را متوقف کند.
                         */
                        qrBitmap?.Dispose();
                        qrBitmap = null;
                    }

                    var title =
                        "ژتون غذا";

                    var printDate =
                        "تاریخ چاپ: " +
                        DateTime.Now
                            .ToPersianDateTime();

                    var userInfo =
                        "کاربر: " +
                        (fullName ?? "-");

                    var mealType =
                        "وعده: " +
                        (detail.Meal?.Title ??
                         "-");

                    var mealDetails =
                        $"جزئیات: {detail.Day?.Title ?? "-"} | " +
                        $"{detail.MainFood?.Title ?? "-"} | " +
                        $"{detail.Dessert?.Title ?? "-"} | " +
                        $"{detail.SideDish?.Title ?? "-"}";

                    var tokenNo =
                        "شماره: " +
                        detail.Id;

                    Action<object, PrintPageEventArgs>
                        printerPage =
                            (sender, e) =>
                            {
                                var g =
                                    e.Graphics;

                                using var font =
                                    new System.Drawing.Font(
                                        "Tahoma",
                                        10);

                                using var boldFont =
                                    new System.Drawing.Font(
                                        "Tahoma",
                                        11,
                                        FontStyle.Bold);

                                using var titleFont =
                                    new System.Drawing.Font(
                                        "Tahoma",
                                        14,
                                        FontStyle.Bold);

                                var pageWidth =
                                    e.PageBounds.Width;

                                const int margin = 1;

                                var contentWidth =
                                    pageWidth -
                                    2 * margin;

                                using var rightAlign =
                                    new StringFormat(
                                        StringFormatFlags
                                            .DirectionRightToLeft);

                                using var centerAlign =
                                    new StringFormat
                                    {
                                        Alignment =
                                            StringAlignment
                                                .Center,
                                        LineAlignment =
                                            StringAlignment
                                                .Center
                                    };

                                var startY =
                                    margin;

                                var offset =
                                    0;

                                g.DrawString(
                                    title,
                                    titleFont,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        35),
                                    centerAlign);

                                offset += 35;

                                g.DrawLine(
                                    Pens.Black,
                                    margin,
                                    startY + offset,
                                    pageWidth - margin,
                                    startY + offset);

                                offset += 10;

                                g.DrawString(
                                    tokenNo,
                                    boldFont,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        20),
                                    rightAlign);

                                offset += 20;

                                g.DrawString(
                                    printDate,
                                    font,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        20),
                                    rightAlign);

                                offset += 20;

                                g.DrawString(
                                    userInfo,
                                    font,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        20),
                                    rightAlign);

                                offset += 20;

                                g.DrawString(
                                    mealType,
                                    font,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        20),
                                    rightAlign);

                                offset += 20;

                                g.DrawString(
                                    mealDetails,
                                    font,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        60),
                                    rightAlign);

                                offset += 55;

                                if (qrBitmap != null)
                                {
                                    const int qrSize =
                                        120;

                                    var qrX =
                                        margin +
                                        Math.Max(
                                            0,
                                            (contentWidth -
                                             qrSize) / 2);

                                    g.DrawImage(
                                        qrBitmap,
                                        new Rectangle(
                                            qrX,
                                            startY + offset,
                                            qrSize,
                                            qrSize));

                                    offset +=
                                        qrSize + 4;

                                    g.DrawString(
                                        "برای تحویل غذا اسکن شود",
                                        font,
                                        Brushes.Black,
                                        new RectangleF(
                                            margin,
                                            startY + offset,
                                            contentWidth,
                                            22),
                                        centerAlign);

                                    offset += 25;
                                }

                                g.DrawLine(
                                    Pens.Black,
                                    margin,
                                    startY + offset,
                                    pageWidth - margin,
                                    startY + offset);

                                offset += 10;

                                g.DrawString(
                                    "نوش جان!",
                                    boldFont,
                                    Brushes.Black,
                                    new RectangleF(
                                        margin,
                                        startY + offset,
                                        contentWidth,
                                        25),
                                    centerAlign);

                                offset += 25;

                                g.DrawRectangle(
                                    Pens.Black,
                                    margin,
                                    margin,
                                    pageWidth -
                                    2 * margin,
                                    offset);
                            };

                    using var pd =
                        new PrintDocument();

                    pd.PrinterSettings.PrinterName =
                        printerName;

                    pd.PrintPage +=
                        (s, e) =>
                            printerPage(
                                s,
                                e);

                    pd.Print();

                    qrBitmap?.Dispose();

                    printedCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;

                    errors.Add(
                        $"ژتون {foodReserveDetaileId}: {ex.Message}");
                }
                finally
                {
                    /*
                     * این قفل فقط برای جلوگیری از چاپ هم‌زمان است؛
                     * پس بعد از پایان عملیات باید آزاد شود تا چاپ مجدد ممکن باشد.
                     */
                    _printedFoodReserveDetailIds
                        .TryRemove(
                            foodReserveDetaileId,
                            out _);
                }
            }

            if (printedCount <= 0)
            {
                return new BaseResult(
                    false,
                    errors.Any()
                        ? string.Join(
                            " | ",
                            errors)
                        : "هیچ ژتونی چاپ نشد.");
            }

            return new BaseResult(
                true,
                failedCount > 0
                    ? $"{printedCount} ژتون چاپ شد و {failedCount} مورد ناموفق بود."
                    : $"{printedCount} ژتون با موفقیت چاپ شد.")
            {
                Model = new
                {
                    PrintedCount =
                        printedCount,

                    FailedCount =
                        failedCount,

                    Errors =
                        errors
                }
            };
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
