using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly ILogger<FoodReserveManager> _printLogger;
        private readonly string _configuredPrinterName;

        public FoodReserveManager(
            DbContexts context,
            IHttpContextAccessor httpContextAccessor,
            IFoodReserveDetailManager foodReserveDetailManager,
            IOptions<PrinterOptions> printerOptions,
            ILogger<FoodReserveManager> printLogger) : base(context, httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            Session = httpContextAccessor.HttpContext?.Session;
            _foodReserveDetailManager = foodReserveDetailManager;
            _printLogger = printLogger;

            // اولویت با تنظیمات برنامه است؛ نام قدیمی فقط برای سازگاری حفظ شده.
            var name = printerOptions?.Value?.FoodTokenPrinterName;
            _configuredPrinterName = string.IsNullOrWhiteSpace(name)
                ? "RONGTA 58mm Series Printer(2)"
                : name.Trim();
        }

        public BaseResult AddToReserveAndReserveDetail(FoodReserveDTO model)
        {
            var now = FoodTimingRules.GetIranNow();

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

        /// <summary>
        /// ثبت یا ویرایش رزرو هفتگی با یک SaveChanges در Context اصلی.
        /// رکوردهای روزهایی که از مهلت رزرو گذشته‌اند اصلاً در ChangeTracker
        /// برای حذف/ویرایش قرار نمی‌گیرند و شماره ژتونشان حفظ می‌شود.
        /// </summary>
        public BaseResult SaveWeeklyReservation(
            FoodReserveDTO model,
            IReadOnlyCollection<(long DayId, long MealId)> editableSlots)
        {
            if (model == null || model.UserId <= 0 ||
                model.Details == null || model.Details.Count == 0 ||
                editableSlots == null || editableSlots.Count == 0)
            {
                return new BaseResult(false, "اطلاعات روزهای قابل رزرو کامل نیست.");
            }

            var slots = editableSlots.ToHashSet();
            if (model.Details.Any(x => !slots.Contains((x.DayId, x.MealId))) ||
                model.Details
                    .GroupBy(x => new { x.DayId, x.MealId })
                    .Any(x => x.Count() > 1))
            {
                return new BaseResult(
                    false, "رزرو شامل روز غیرمجاز یا وعده تکراری است.");
            }

            try
            {
                var existing = UOW.FoodReserve.Get(
                    x => x.UserId == model.UserId &&
                         x.WeekStartDate == model.WeekStartDate.Date &&
                         x.WeekEndDate == model.WeekEndDate.Date)
                    .FirstOrDefault();

                var newDetails = model.Details.Select(item => new FoodReserveDetail
                {
                    DayId = item.DayId,
                    MealId = item.MealId,
                    MainFoodId = item.MainFoodId,
                    DessertId = item.DessertId,
                    SideDishId = item.SideDishId
                }).ToList();

                if (existing == null)
                {
                    var now = FoodTimingRules.GetIranNow();
                    UOW.FoodReserve.Add(new FoodReserve
                    {
                        UserId = model.UserId,
                        WeekStartDate = model.WeekStartDate.Date,
                        WeekEndDate = model.WeekEndDate.Date,
                        Year = now.Year,
                        Month = now.Month,
                        CreatedAt = now,
                        Details = newDetails
                    });
                }
                else
                {
                    // برخلاف حذف رزرو اصلی و ایجاد مجدد آن، فقط جزئیات روزها و وعده‌های
                    // قابل‌ویرایش تغییر می‌کنند. موارد قفل‌شده همان شناسه را دارند.
                    var editableExisting = UOW.FoodReserveDetail.Get(
                        x => x.FoodReserveId == existing.Id)
                        .ToList()
                        .Where(x => slots.Contains((x.DayId, x.MealId)))
                        .ToList();

                    var incoming = newDetails.ToDictionary(
                        x => (x.DayId, x.MealId));

                    foreach (var oldDetail in editableExisting)
                    {
                        var key = (oldDetail.DayId, oldDetail.MealId);
                        if (incoming.TryGetValue(key, out var incomingDetail))
                        {
                            // در صورت ویرایش فقط محتوای غذا به‌روزرسانی شود؛
                            // شناسه ژتون برای همان روز/وعده ثابت می‌ماند.
                            if (oldDetail.MainFoodId != incomingDetail.MainFoodId ||
                                oldDetail.DessertId != incomingDetail.DessertId ||
                                oldDetail.SideDishId != incomingDetail.SideDishId)
                            {
                                oldDetail.MainFoodId = incomingDetail.MainFoodId;
                                oldDetail.DessertId = incomingDetail.DessertId;
                                oldDetail.SideDishId = incomingDetail.SideDishId;
                                UOW.FoodReserveDetail.Update(oldDetail);
                            }

                            incoming.Remove(key);
                        }
                        else
                        {
                            // صرفاً وعده حذف‌شده از فرم و هنوز قابل‌ویرایش.
                            UOW.FoodReserveDetail.Remove(oldDetail);
                        }
                    }

                    foreach (var newDetail in incoming.Values)
                    {
                        newDetail.FoodReserveId = existing.Id;
                        UOW.FoodReserveDetail.Add(newDetail);
                    }
                }

                // همه حذف‌ها و درج‌ها در یک SaveChanges دیتابیس انجام می‌شوند.
                // EF Core تغییرات یک SaveChanges را به صورت تراکنشی ذخیره می‌کند.
                var success = UOW.Commit();
                return new BaseResult(
                    success,
                    success
                        ? "برنامه غذایی با موفقیت ثبت شد."
                        : "ذخیره رزرو انجام نشد؛ رزرو قبلی را بررسی کنید.");
            }
            catch (Exception ex)
            {
                _printLogger.LogError(
                    ex,
                    "خطا در ذخیره اتمیک رزرو هفتگی. UserId: {UserId}, Week: {Week}",
                    model.UserId, model.WeekStartDate);
                return new BaseResult(false,
                    "ثبت رزرو با خطا همراه بود؛ لطفاً دوباره بررسی کنید.");
            }
        }

        public BaseResult DeleteDayReservation(
            long userId, DateTime weekStartDate, long dayId)
        {
            if (userId <= 0 || dayId <= 0)
                return new BaseResult(false, "شناسه کاربر یا روز معتبر نیست.");

            try
            {
                var reservation = UOW.FoodReserve.Get(x =>
                    x.UserId == userId &&
                    x.WeekStartDate == weekStartDate.Date)
                    .FirstOrDefault();

                if (reservation == null)
                    return new BaseResult(false, "رزرو این هفته پیدا نشد.");

                var details = UOW.FoodReserveDetail.Get(
                    x => x.FoodReserveId == reservation.Id).ToList();

                var forDay = details.Where(x => x.DayId == dayId).ToList();
                if (forDay.Count == 0)
                    return new BaseResult(false, "برای این روز رزروی ثبت نشده است.");

                UOW.FoodReserveDetail.RemoveRange(forDay);

                if (details.Count == forDay.Count)
                    UOW.FoodReserve.Remove(reservation);

                var success = UOW.Commit();
                return new BaseResult(success,
                    success
                        ? "رزرو روز انتخاب‌شده حذف شد."
                        : "حذف رزرو انجام نشد.");
            }
            catch (Exception ex)
            {
                _printLogger.LogError(
                    ex,
                    "خطا در حذف رزرو یک روز. UserId: {UserId}, DayId: {DayId}",
                    userId, dayId);
                return new BaseResult(false, "حذف رزرو با خطا همراه بود.");
            }
        }

        public List<FoodReserveDetail> GetFoodReserveDetail(long userId)
        {
            var today = FoodTimingRules.GetIranNow().Date;

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
            var today = FoodTimingRules.GetIranNow().Date;

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
                    .Include(x => x.SideDish)
            ).ToList();

            // ساخت لیست خروجی بر اساس جزئیات
            var list = reservDetail.Select(d => new WeeklyFoodReserveDTO
            {
                Date = reserve.WeekStartDate.AddDays(d.Day.Code - 1),
                DayTitle = d.Day?.Title ?? "-",
                FoodReserveDetaileId = d.Id,
                MealId = d.MealId,
                MealTitle = d.Meal?.Title ?? "-",
                MainFood = d.MainFood?.Title ?? "-",
                Dessert = d.Dessert?.Title ?? "-",
                SideDish = d.SideDish?.Title ?? "-"
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

            var printerName = _configuredPrinterName;
            var hostName = Environment.MachineName;
            var hostUser = Environment.UserName;

            _printLogger.LogInformation(
                "شروع ارسال ژتون به چاپگر سرور. Server: {Server}, ProcessUser: {ProcessUser}, Printer: {Printer}, UserId: {UserId}, DetailCount: {DetailCount}",
                hostName, hostUser, printerName, userId, foodReserveDetaileIds.Count);

            try
            {
                if (!OperatingSystem.IsWindows())
                    throw new PlatformNotSupportedException(
                        "چاپ مستقیم با PrintDocument فقط روی ویندوز پشتیبانی می‌شود؛ از چاپ مرورگر استفاده کنید.");

                EnsurePrinterIsValid(printerName);
            }
            catch (Exception ex)
            {
                _printLogger.LogError(
                    ex,
                    "چاپگر سرور آماده نیست. Server: {Server}, ProcessUser: {ProcessUser}, Printer: {Printer}",
                    hostName, hostUser, printerName);
                return new BaseResult(false, ex.Message);
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

                Bitmap qrBitmap = null;
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
                    catch (Exception qrException)
                    {
                        // خطای QR قدیمی مانع چاپ رسید نمی‌شود، اما حتماً ثبت می‌شود.
                        _printLogger.LogWarning(
                            qrException,
                            "تولید QR قدیمی ژتون ناموفق بود. DetailId: {DetailId}, UserId: {UserId}",
                            foodReserveDetaileId, userId);
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

                    using var pd = new PrintDocument();
                    pd.PrinterSettings.PrinterName = printerName;
                    pd.PrinterSettings.PrintToFile = false;
                    pd.PrintController = new StandardPrintController();
                    pd.DocumentName = $"FoodTicket-{foodReserveDetaileId}";

                    var printPageExecuted = false;
                    pd.PrintPage += (s, e) =>
                    {
                        printPageExecuted = true;
                        printerPage(s, e);
                    };

                    pd.Print();

                    if (!printPageExecuted)
                        throw new InvalidOperationException(
                            "رویداد PrintPage اجرا نشد؛ هیچ محتوایی به صف چاپ تحویل داده نشد.");

                    // بازگشت Print() یعنی تحویل به چاپگر/Spooler؛
                    // چاپ فیزیکی کاغذ از داخل این API قابل تأیید نیست.
                    _printLogger.LogInformation(
                        "ژتون به صف چاپ ارسال شد (نه تأیید چاپ روی کاغذ). Server: {Server}, Printer: {Printer}, DetailId: {DetailId}, UserId: {UserId}",
                        hostName, printerName, foodReserveDetaileId, userId);

                    printedCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _printLogger.LogError(
                        ex,
                        "ارسال ژتون به صف چاپ ناموفق بود. Server: {Server}, Printer: {Printer}, DetailId: {DetailId}, UserId: {UserId}",
                        hostName, printerName, foodReserveDetaileId, userId);
                    errors.Add(
                        $"ژتون {foodReserveDetaileId}: {ex.Message}");
                }
                finally
                {
                    qrBitmap?.Dispose();
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

            var queueMessage =
                $"{printedCount} ژتون به صف چاپ «{printerName}» روی سیستم «{hostName}» ارسال شد. " +
                "این پیام تأیید خروج کاغذ نیست؛ صف چاپ و دستگاه را بررسی کنید.";

            if (failedCount > 0)
                queueMessage += $" {failedCount} مورد نیز ناموفق بود.";

            return new BaseResult(true, queueMessage)
            {
                Model = new
                {
                    QueuedCount = printedCount,
                    FailedCount = failedCount,
                    PrinterName = printerName,
                    PrintHost = hostName,
                    PrintProcessUser = hostUser,
                    Errors = errors
                }
            };
        }

        private static void EnsurePrinterIsValid(string printerName)
        {
            var installed = PrinterSettings.InstalledPrinters
                .Cast<string>()
                .ToList();

            if (!installed.Any(p =>
                    string.Equals(p, printerName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"چاپگر «{printerName}» برای حساب اجرای برنامه روی سرور «{Environment.MachineName}» نصب/قابل مشاهده نیست. " +
                    "نام چاپگر یا حساب IIS را بررسی کنید، یا از گزینه چاپ با پرینتر این رایانه استفاده کنید.");
            }

            var ps = new PrinterSettings { PrinterName = printerName };
            if (!ps.IsValid)
                throw new InvalidOperationException(
                    $"چاپگر «{printerName}» از دید فرایند وب معتبر نیست (IsValid=false). " +
                    "درایور، پورت و حساب اجرای برنامه را بررسی کنید.");
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
