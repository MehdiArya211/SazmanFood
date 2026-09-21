using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.QoutaPerson;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.SessionServices;

namespace BLL;

public class QoutaPersonManager : Manager<QoutaPerson, ApplicationContext>, IQoutaPersonManager
{
    protected readonly IHttpContextAccessor httpContextAccessor;
    protected readonly ISession Session;

    public QoutaPersonManager(DbContexts _Context, IHttpContextAccessor httpContextAccessor) : base(_Context, httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        Session = httpContextAccessor.HttpContext.Session;
    }

    public BaseResult Create(QoutaPersonCreateDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null)
                return new BaseResult(false, "اطلاعات ارسالی نامعتبر است");

            var qoutaPerson = new QoutaPerson()
            {
                QoutaAllocationId = model.QoutaAllocationId,
                FName = model.FName,
                LName = model.LName,
                RegUserId = user.Id,
                PersonalCode = model.PersonalCode,
                PersonalTypeId = model.PersonalTypeId,
                MainFoodId = model.FoodId,
                PersonId = model.PersonId,
                CreateDate = DateTime.Now,
                IsDeleted = false,
            };

            return base.Create(qoutaPerson);
        }
        catch
        {
            return new BaseResult(false, "ثبت اطلاعات با خطا همراه بوده است");
        }
    }

    public DataTableResponseDTO<QoutaPersonDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, long id)
    {
        var user = Session.GetUser();

        return UOW.QoutaPerson.GetDataTableDTO(searchData, user.Id, id);
    }


    #region Sahmiye AI

    private static int CalcCountByMealTitleAi(string mealTitle, FoodSource src, int estedad)
    {
        double percent =
            mealTitle == "صبحانه" ? src.PercentBreakfast :
            mealTitle.Contains("ناهار") ? src.PercentLunch :
            mealTitle.Contains("شام") ? src.PercentDinner :
            0;

        return (int)Math.Floor((percent * estedad) / 100.0);
    }

    #endregion

    private static int CalcCountByMealTitle(string mealTitle, FoodSource src, int estedad)
    {
        double percent =
            mealTitle == "صبحانه" ? src.PercentBreakfast :
            mealTitle.Contains("ناهار") ? src.PercentLunch :
            mealTitle.Contains("شام") ? src.PercentDinner :
            0;

        return (int)Math.Floor((percent * estedad) / 100.0);
    }

    public List<QoutaPersonDataTableDTO> GetPersonListWithQouataAllocation(long qoutaAllocationId)
    {
        var personList = UOW.QoutaPerson
            .GetDTO<QoutaPersonDataTableDTO>(
                QoutaPersonDataTableDTO.Selector,
                x => x.QoutaAllocationId == qoutaAllocationId && x.IsDeleted == false)
            .ToList();

        return personList;
    }

    private static (string FName, string LName) SplitFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ("-", "-");

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
            return (parts[0], "-");

        return (parts[0], string.Join(" ", parts.Skip(1)));
    }

    #region Office User And Guest Food



    private string GenerateUniqueDeliveryCode()
    {
        string code;

        do
        {
            code = Random.Shared.Next(100000, 999999).ToString();
        }
        while (UOW.QoutaPerson.Any(x =>
            x.DeliveryCode == code &&
            x.IsDeleted == false &&
            x.IsDelivered != true));

        return code;
    }

    private static string GenerateDeliveryCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    public BaseResult ChangePersonFood(ChangePersonFoodDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || model.QoutaPersonId <= 0 || model.MainFoodId <= 0)
                return new BaseResult(false, "اطلاعات ارسالی ناقص است");

            var qoutaPerson = UOW.QoutaPerson
                .Get(x => x.Id == model.QoutaPersonId && x.IsDeleted == false, null, null, null,
                    x => x.Include(i => i.QoutaAllocation))
                .SingleOrDefault();

            if (qoutaPerson == null)
                return new BaseResult(false, "غذای ثبت شده یافت نشد");

            if (user.PersonId == null || qoutaPerson.PersonId != user.PersonId)
                return new BaseResult(false, "شما مجاز به تغییر این غذا نیستید");

            if (qoutaPerson.IsDelivered == true)
                return new BaseResult(false, "این غذا تحویل شده و قابل تغییر نیست");

            if (qoutaPerson.QoutaAllocation == null)
                return new BaseResult(false, "سهمیه غذا یافت نشد");

            if (qoutaPerson.QoutaAllocation.IsFinalized == true)
                return new BaseResult(false, "سهمیه نهایی شده و قابل تغییر نیست");

            if (qoutaPerson.QoutaAllocation.AllowPersonChange != true)
                return new BaseResult(false, "امکان تغییر غذا توسط کاربر فعال نیست");

            if (qoutaPerson.QoutaAllocation.RegisterDeadline.HasValue &&
                DateTime.Now > qoutaPerson.QoutaAllocation.RegisterDeadline.Value)
                return new BaseResult(false, "مهلت تغییر غذا به پایان رسیده است");

            qoutaPerson.MainFoodId = model.MainFoodId;
            qoutaPerson.DessertId = model.DessertId;
            qoutaPerson.SideDishId = model.SideDishId;
            qoutaPerson.LastEditUserId = user.Id;
            qoutaPerson.LastEditDate = DateTime.Now;

            return base.Update(qoutaPerson);
        }
        catch
        {
            return new BaseResult(false, "تغییر غذا با خطا همراه بوده است");
        }
    }

    public BaseResult DeliverFoodByCode(DeliverFoodByCodeDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || string.IsNullOrWhiteSpace(model.DeliveryCode))
                return new BaseResult(false, "کد تحویل نامعتبر است");

            var today = DateTime.Today;

            var qoutaPerson = UOW.QoutaPerson
                .Get(x => x.DeliveryCode == model.DeliveryCode &&
                          x.IsDeleted == false, null, null, null,
                    x => x.Include(i => i.QoutaAllocation)
                          .Include(i => i.MainFood)
                          .Include(i => i.FoodTokenType)
                          .Include(i => i.FoodReceiverType))
                .SingleOrDefault();

            if (qoutaPerson == null)
                return new BaseResult(false, "ژتون مورد نظر یافت نشد");

            if (qoutaPerson.QoutaAllocation == null)
                return new BaseResult(false, "اطلاعات سهمیه ژتون یافت نشد");

            if (qoutaPerson.QoutaAllocation.QoutaAllocationDate.Date != today)
                return new BaseResult(false, "این ژتون مربوط به امروز نیست");

            if (qoutaPerson.QoutaAllocation.MealId != model.MealId)
                return new BaseResult(false, "این ژتون مربوط به این وعده غذایی نیست");

            if (qoutaPerson.IsDelivered == true)
                return new BaseResult(false, "این ژتون قبلاً استفاده شده است");

            qoutaPerson.IsDelivered = true;
            qoutaPerson.DeliveredDate = DateTime.Now;
            qoutaPerson.DeliveredUserId = user.Id;
            qoutaPerson.LastEditUserId = user.Id;
            qoutaPerson.LastEditDate = DateTime.Now;

            var res = base.Update(qoutaPerson);

            if (!res.Status)
                return res;

            return new BaseResult(true, "غذا با موفقیت تحویل شد")
            {
                Model = new
                {
                    qoutaPerson.FName,
                    qoutaPerson.LName,
                    FoodTitle = qoutaPerson.MainFood?.Title,
                    ReceiverType = qoutaPerson.FoodReceiverType?.Title,
                    TokenType = qoutaPerson.FoodTokenType?.Title
                }
            };
        }
        catch
        {
            return new BaseResult(false, "تحویل غذا با خطا همراه بوده است");
        }
    }
    /// <summary>
    /// QR را اعتبارسنجی و ژتون را به‌صورت اتمیک مصرف می‌کند.
    /// شرط IsDelivered در خود دستور Update مانع مصرف هم‌زمان یا دوباره می‌شود.
    /// </summary>
    public BaseResult DeliverFoodByQr(
        string deliveryCode,
        string deliveryHash,
        long mealId)
    {
        try
        {
            deliveryCode = deliveryCode?.Trim();
            deliveryHash = deliveryHash?.Trim();

            if (string.IsNullOrWhiteSpace(deliveryCode) ||
                string.IsNullOrWhiteSpace(deliveryHash) ||
                mealId <= 0)
            {
                return new BaseResult(false, "اطلاعات QR ژتون معتبر نیست");
            }

            var token = UOW.QoutaPerson
                .Get(
                    x => x.DeliveryCode == deliveryCode &&
                         x.DeliveryHash == deliveryHash &&
                         x.IsDeleted == false,
                    null,
                    null,
                    null,
                    x => x.Include(i => i.QoutaAllocation)
                          .Include(i => i.MainFood)
                          .Include(i => i.FoodTokenType)
                          .Include(i => i.FoodReceiverType))
                .SingleOrDefault();

            if (token == null)
                return new BaseResult(false, "ژتون مورد نظر یافت نشد یا QR معتبر نیست");

            if (token.QoutaAllocation == null)
                return new BaseResult(false, "اطلاعات سهمیه ژتون یافت نشد");

            if (token.QoutaAllocation.QoutaAllocationDate.Date != DateTime.Today)
                return new BaseResult(false, "این ژتون مربوط به امروز نیست");

            if (token.QoutaAllocation.MealId != mealId)
                return new BaseResult(false, "این ژتون مربوط به این وعده غذایی نیست");

            if (token.IsDelivered)
            {
                return new BaseResult(false, "این ژتون قبلاً استفاده شده است")
                {
                    Model = new
                    {
                        token.DeliveredDate
                    }
                };
            }

            var currentUser = Session?.GetUser();
            var deliveredAt = DateTime.Now;

            var affectedRows = UpdateWithCommit(
                x => x.Id == token.Id &&
                     x.IsDeleted == false &&
                     x.IsDelivered == false,
                x => new QoutaPerson
                {
                    IsDelivered = true,
                    DeliveredDate = deliveredAt,
                    DeliveredUserId = currentUser != null ? currentUser.Id : null,
                    LastEditUserId = currentUser != null ? currentUser.Id : null,
                    LastEditDate = deliveredAt
                });

            if (affectedRows != 1)
                return new BaseResult(false, "این ژتون قبلاً استفاده شده است");

            return new BaseResult(true, "ژتون معتبر است؛ غذا با موفقیت تحویل شد")
            {
                Model = new
                {
                    FullName = $"{token.FName} {token.LName}".Trim(),
                    FoodTitle = token.MainFood?.Title,
                    ReceiverType = token.FoodReceiverType?.Title,
                    TokenType = token.FoodTokenType?.Title,
                    DeliveredDate = deliveredAt
                }
            };
        }
        catch
        {
            return new BaseResult(false, "بررسی و ثبت QR ژتون با خطا همراه بود");
        }
    }


    public DataTableResponseDTO<MyFoodDataTableDTO> GetMyFoodDataTableDTO(DataTableSearchDTO searchData, long personId)
    {
        var query = UOW.QoutaPerson
            .GetDTO(
                MyFoodDataTableDTO.Selector,
                x => x.PersonId == personId && x.IsDeleted == false)
            .ToList();

        return new DataTableResponseDTO<MyFoodDataTableDTO>
        {
            draw = searchData.draw,
            recordsTotal = query.Count,
            recordsFiltered = query.Count,
            data = query
                .Skip(searchData.start)
                .Take(searchData.length)
                .ToList()
        };
    }
    /// <summary>
    /// بررسی دسترسی مدیر اداری برای ثبت غذا
    /// </summary>
    private BaseResult CheckAdministrativeManagerAccess(QoutaAllocation allocation)
    {
        var user = Session.GetUser();

        if (user == null)
            return new BaseResult(false, "اطلاعات کاربر جاری یافت نشد");

        if (allocation == null)
            return new BaseResult(false, "سهمیه مورد نظر یافت نشد");

        if (!user.RoleId.HasValue)
            return new BaseResult(false, "نقش کاربر جاری مشخص نیست");

        var roleTitle = UOW.Roles
            .Get(x => x.Id == user.RoleId.Value && x.IsDeleted == false && x.IsEnabled)
            .Select(x => x.Title)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(roleTitle))
            return new BaseResult(false, "نقش کاربر جاری یافت نشد یا غیرفعال است");

        roleTitle = roleTitle.Trim();

        var isAdmin =
            roleTitle == "مدیر سیستم" ||
            roleTitle == "مدیر کل سیستم" ||
            roleTitle == "ادمین" ||
            roleTitle.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        if (isAdmin)
            return new BaseResult(true, "دسترسی مجاز است");

        if (roleTitle != "مدیر اداری")
            return new BaseResult(false, "شما نقش مدیر اداری را ندارید");

        if (user.OrganGarrisonId == null)
            return new BaseResult(false, "یگان پادگان کاربر جاری مشخص نیست");

        if (allocation.OrganGarrisonId != user.OrganGarrisonId)
            return new BaseResult(false, "شما مجاز به ثبت غذا برای این یگان نیستید");

        return new BaseResult(true, "دسترسی مجاز است");
    }


    #endregion

    /// <summary>
    /// بررسی می‌کند برای کد پرسنلی، سهمیه امروز یا آینده ثبت شده باشد.
    /// </summary>
    public bool HasActiveQuota(string personalCode)
    {
        personalCode = personalCode?.Trim();

        if (string.IsNullOrWhiteSpace(personalCode))
            return false;

        var today = DateTime.Today;

        return UOW.QoutaPerson.Any(x =>
            x.PersonalCode == personalCode &&
            x.IsDeleted == false &&
            x.QoutaAllocation != null &&
            x.QoutaAllocation.IsDeleted == false &&
            x.QoutaAllocation.QoutaAllocationDate >= today);
    }


    /// <summary>
    /// تاریخ و وعده‌های دارای سهمیه پرسنل را برای جلوگیری از رزرو خارج از سهمیه برمی‌گرداند.
    /// </summary>
    public IReadOnlyCollection<(DateTime Date, long MealId)> GetReservableSlots(
        string personalCode,
        DateTime fromDate,
        DateTime toDate)
    {
        personalCode = personalCode?.Trim();
        fromDate = fromDate.Date;
        toDate = toDate.Date;

        if (string.IsNullOrWhiteSpace(personalCode) || fromDate > toDate)
            return Array.Empty<(DateTime Date, long MealId)>();

        return UOW.QoutaPerson
            .Get(
                x => x.PersonalCode == personalCode &&
                     x.IsDeleted == false &&
                     x.QoutaAllocation != null &&
                     x.QoutaAllocation.IsDeleted == false &&
                     x.QoutaAllocation.QoutaAllocationDate >= fromDate &&
                     x.QoutaAllocation.QoutaAllocationDate <= toDate,
                null,
                null,
                null,
                x => x.Include(i => i.QoutaAllocation))
            .ToList()
            .Select(x => (
                x.QoutaAllocation.QoutaAllocationDate.Date,
                x.QoutaAllocation.MealId))
            .Distinct()
            .ToList();
    }

}