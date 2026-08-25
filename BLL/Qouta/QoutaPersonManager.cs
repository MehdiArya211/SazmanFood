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

    public OrganSahmiyeDTO GetSahmiye(long qoutaAllocationId)
    {
        var model = new OrganSahmiyeDTO();

        var allocation = UOW.QoutaAllocation.Get(
            m => m.Id == qoutaAllocationId && m.IsDeleted == false,
            null,
            null,
            null,
            m => m.Include(x => x.Meal)
                  .Include(x => x.OrganGarrison)
                  .ThenInclude(x => x.OrganGarrisonType))
            .SingleOrDefault();

        if (allocation?.Meal == null || allocation.OrganGarrison == null)
            return model;

        model.MealTitle = allocation.Meal.Title;

        var yeganTypeId = allocation.OrganGarrison.OrganGarrisonTypeId;

        var percentageKadr = UOW.FoodSource
            .Get(m => m.PersonalTypeId == 1 && m.YeganTypeId == yeganTypeId)
            .ToList();

        var percentageVazife = UOW.FoodSource
            .Get(m => m.PersonalTypeId == 2 && m.YeganTypeId == yeganTypeId)
            .ToList();

        // کادر
        foreach (var item in percentageKadr)
        {
            model.EstedadeKadr = CalcCountByMealTitle(allocation.Meal.Title, item, allocation.OrganGarrison.EstedadKadr);
        }

        // وظیفه
        foreach (var item in percentageVazife)
        {
            model.EstedadeVazife = CalcCountByMealTitle(allocation.Meal.Title, item, allocation.OrganGarrison.EstedadVazife);
        }

        return model;
    }

    #region Sahmiye AI

    public OrganSahmiyeDTO GetSahmiyeAi(long qoutaAllocationId)
    {
        var model = new OrganSahmiyeDTO();

        var allocation = UOW.QoutaAllocation
            .Get(x => x.Id == qoutaAllocationId && x.IsDeleted == false, null, null, null,
                q => q.Include(i => i.Meal)
                      .Include(i => i.OrganGarrison)
                      .ThenInclude(i => i.OrganGarrisonType))
            .SingleOrDefault();

        if (allocation?.Meal == null || allocation.OrganGarrison == null)
            return model;

        model.MealTitle = allocation.Meal.Title;

        var yeganTypeId = allocation.OrganGarrison.OrganGarrisonTypeId;

        var srcKadr = UOW.FoodSource
            .Get(x => x.PersonalTypeId == 1 && x.YeganTypeId == yeganTypeId)
            .SingleOrDefault();

        var srcVazife = UOW.FoodSource
            .Get(x => x.PersonalTypeId == 2 && x.YeganTypeId == yeganTypeId)
            .SingleOrDefault();

        if (srcKadr != null)
            model.EstedadeKadr = CalcCountByMealTitleAi(allocation.Meal.Title, srcKadr, allocation.OrganGarrison.EstedadKadr);

        if (srcVazife != null)
            model.EstedadeVazife = CalcCountByMealTitleAi(allocation.Meal.Title, srcVazife, allocation.OrganGarrison.EstedadVazife);

        return model;
    }

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

    public BaseResult CreateBulk(QoutaPersonBulkCreateDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || model.QoutaAllocationId <= 0)
                return new BaseResult(false, "اطلاعات ارسالی نامعتبر است");

            if (model.Items == null || model.Items.Count == 0)
                return new BaseResult(false, "هیچ پرسنلی انتخاب نشده است");

            // پاکسازی آیتم‌ها
            var items = model.Items
                .Where(x => x.PersonId > 0 && x.FoodId > 0)
                .GroupBy(x => x.PersonId)
                .Select(g => g.First())
                .ToList();

            if (items.Count == 0)
                return new BaseResult(false, "هیچ پرسنلی انتخاب نشده یا غذا انتخاب نشده است");

            // در این DTO باید PersonId واقعاً شناسه جدول Person باشد
            var personIds = items
                .Select(x => x.PersonId)
                .Distinct()
                .ToList();

            // سهمیه مجاز
            var sahmiye = GetSahmiye(model.QoutaAllocationId);
            var maxKadr = sahmiye.EstedadeKadr;
            var maxVazife = sahmiye.EstedadeVazife;

            var allocation = UOW.QoutaAllocation
                .Get(x => x.Id == model.QoutaAllocationId && x.IsDeleted == false, null, null, null,
                    x => x.Include(i => i.OrganGarrison))
                .SingleOrDefault();

            if (allocation == null)
                return new BaseResult(false, "سهمیه بندی مورد نظر یافت نشد");


            if (allocation.AllowOfficeUserRegister != true)
                return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

            var accessResult = CheckAdministrativeManagerAccess(allocation);
            if (!accessResult.Status)
                return accessResult;

            var persons = UOW.Person
                .Get(x => personIds.Contains(x.Id) && x.IsDeleted == false)
                .Select(x => new
                {
                    x.Id,
                    x.PersonTypeId,
                    x.FullName,
                    x.PersonCode,
                    x.OrganGarrisonId
                })
                .ToList();

            if (persons.Count == 0)
                return new BaseResult(false, "پرسنل انتخاب‌شده یافت نشد");

            var notFoundIds = personIds.Except(persons.Select(x => x.Id)).ToList();
            if (notFoundIds.Any())
                return new BaseResult(false, "بعضی از پرسنل انتخاب‌شده در سیستم یافت نشدند");

            var noCode = persons.Where(x => x.PersonCode == null).ToList();
            if (noCode.Any())
                return new BaseResult(false, "برای برخی پرسنل کد پرسنلی ثبت نشده است");

            var personCodes = persons
                .Select(x => x.PersonCode.Value.ToString())
                .Distinct()
                .ToList();

            var alreadyExistsPersonIds = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          x.PersonId.HasValue &&
                          personIds.Contains(x.PersonId.Value) &&
                          x.IsDeleted == false)
                .Select(x => x.PersonId.Value)
                .ToList();

            var alreadyExistsCodes = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          personCodes.Contains(x.PersonalCode) &&
                          x.IsDeleted == false)
                .Select(x => x.PersonalCode)
                .ToList();

            var newPersons = persons
                .Where(p => !alreadyExistsPersonIds.Contains(p.Id) &&
                            !alreadyExistsCodes.Contains(p.PersonCode.Value.ToString()))
                .ToList();

            if (newPersons.Count == 0)
                return new BaseResult(false, "تمام پرسنل انتخاب‌شده قبلاً برای این سهمیه ثبت شده‌اند");

            var currentCounts = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          x.IsDeleted == false)
                .GroupBy(x => x.PersonalTypeId)
                .Select(g => new { PersonalTypeId = g.Key, Count = g.Count() })
                .ToList();

            var currentKadr = currentCounts.FirstOrDefault(x => x.PersonalTypeId == 1)?.Count ?? 0;
            var currentVazife = currentCounts.FirstOrDefault(x => x.PersonalTypeId == 2)?.Count ?? 0;

            var addKadr = newPersons.Count(x => x.PersonTypeId == 1);
            var addVazife = newPersons.Count(x => x.PersonTypeId == 2);

            if (currentKadr + addKadr > maxKadr)
                return new BaseResult(false, $"ظرفیت کادر تکمیل است. ظرفیت: {maxKadr} ، ثبت شده: {currentKadr}");

            if (currentVazife + addVazife > maxVazife)
                return new BaseResult(false, $"ظرفیت وظیفه تکمیل است. ظرفیت: {maxVazife} ، ثبت شده: {currentVazife}");

            var foodByPersonId = items.ToDictionary(x => x.PersonId, x => x.FoodId);

            var createdCount = 0;

            foreach (var p in newPersons)
            {
                if (!foodByPersonId.TryGetValue(p.Id, out var foodId))
                    return new BaseResult(false, "برای برخی افراد غذا انتخاب نشده است");

                var (fName, lName) = SplitFullName(p.FullName);

                var qoutaPerson = new QoutaPerson
                {
                    QoutaAllocationId = model.QoutaAllocationId,
                    PersonId = p.Id,

                    FName = fName,
                    LName = lName,
                    PersonalCode = p.PersonCode.Value.ToString(),

                    PersonalTypeId = p.PersonTypeId,

                    OrgId = allocation.OrgId,
                    OrgTitle = allocation.OrganGarrison?.Title,
                    OrganGarrisonId = p.OrganGarrisonId,

                    MainFoodId = foodId,

                    // اگر فیلدها را به Entity اضافه کرده‌ای، این مقادیر مناسب هستند
                    FoodReceiverTypeId = 1,
                    FoodTokenTypeId = 1,
                    DeliveryCode = GenerateUniqueDeliveryCode(),

                    RegUserId = user.Id,
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                };

                var res = base.Create(qoutaPerson);
                if (!res.Status)
                    return new BaseResult(false, res.Message);

                createdCount++;
            }

            return new BaseResult(true, $"{createdCount} نفر با موفقیت ثبت شدند");
        }
        catch
        {
            return new BaseResult(false, "ثبت گروهی با خطا همراه بوده است!");
        }
    }

    public BaseResult CreateSingle(QoutaPersonCreateDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || model.QoutaAllocationId <= 0 || model.FoodId == null)
                return new BaseResult(false, "اطلاعات ناقص است");

            var sahmiye = GetSahmiye(model.QoutaAllocationId);
            if (sahmiye == null)
                return new BaseResult(false, "سهمیه مورد نظر یافت نشد");

            var allocation = UOW.QoutaAllocation
                .Get(x => x.Id == model.QoutaAllocationId && x.IsDeleted == false, null, null, null,
                     x => x.Include(i => i.OrganGarrison))
                .SingleOrDefault();

            if (allocation == null)
                return new BaseResult(false, "سهمیه بندی یافت نشد");


            if (allocation.AllowOfficeUserRegister != true)
                return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

            var accessResult = CheckAdministrativeManagerAccess(allocation);
            if (!accessResult.Status)
                return accessResult;


            var currentCount = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          x.PersonalTypeId == model.PersonalTypeId &&
                          x.IsDeleted == false)
                .Count();

            var maxAllowed = model.PersonalTypeId == 1 ? sahmiye.EstedadeKadr : sahmiye.EstedadeVazife;

            if (currentCount >= maxAllowed)
                return new BaseResult(false, $"ظرفیت {(model.PersonalTypeId == 1 ? "کادر" : "وظیفه")} تکمیل است");

            var exists = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          x.PersonId == model.PersonId &&
                          x.IsDeleted == false)
                .Any();

            if (exists)
                return new BaseResult(false, "این پرسنل قبلاً برای این سهمیه ثبت شده است");

            var qoutaPerson = new QoutaPerson
            {
                QoutaAllocationId = model.QoutaAllocationId,
                FName = model.FName,
                LName = model.LName,
                PersonalCode = model.PersonalCode,
                PersonalTypeId = model.PersonalTypeId,
                MainFoodId = model.FoodId.Value,
                OrgId = allocation.OrgId,
                OrgTitle = allocation.OrganGarrison?.Title,
                RegUserId = user.Id,
                CreateDate = DateTime.Now,
                IsDeleted = false,
                PersonId = model.PersonId,

                FoodReceiverTypeId = 1,
                FoodTokenTypeId = 1,
                DeliveryCode = GenerateUniqueDeliveryCode()
            };

            return base.Create(qoutaPerson);
        }
        catch
        {
            return new BaseResult(false, "ثبت با خطا همراه بوده است!");
        }
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

    public BaseResult CreateForOfficeUser(QoutaPersonOfficeCreateDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || model.QoutaAllocationId <= 0 || model.PersonId <= 0 || model.MainFoodId <= 0)
                return new BaseResult(false, "اطلاعات ارسالی ناقص است");

            var allocation = UOW.QoutaAllocation
                .Get(x => x.Id == model.QoutaAllocationId && x.IsDeleted == false, null, null, null,
                    x => x.Include(i => i.OrganGarrison))
                .SingleOrDefault();

            if (allocation == null)
                return new BaseResult(false, "سهمیه مورد نظر یافت نشد");

            if (allocation.AllowOfficeUserRegister != true)
                return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

            var accessResult = CheckAdministrativeManagerAccess(allocation);
            if (!accessResult.Status)
                return accessResult;

            if (allocation.AllowOfficeUserRegister != true)
                return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

            if (allocation.RegisterDeadline.HasValue && DateTime.Now > allocation.RegisterDeadline.Value)
                return new BaseResult(false, "مهلت ثبت غذا برای این سهمیه به پایان رسیده است");

            var person = UOW.Person
                .Get(x => x.Id == model.PersonId && x.IsDeleted == false)
                .SingleOrDefault();

            if (person == null)
                return new BaseResult(false, "پرسنل مورد نظر یافت نشد");

            if (person.PersonCode == null)
                return new BaseResult(false, "برای پرسنل مورد نظر کد پرسنلی ثبت نشده است");

            var exists = UOW.QoutaPerson.Any(x =>
                x.QoutaAllocationId == model.QoutaAllocationId &&
                x.PersonId == model.PersonId &&
                x.IsDeleted == false);

            if (exists)
                return new BaseResult(false, "برای این پرسنل قبلاً غذا ثبت شده است");

            var capacityResult = CheckCapacityForPerson(allocation.Id, person.PersonTypeId, model.FoodTokenTypeId);
            if (!capacityResult.Status)
                return capacityResult;

            var fullName = SplitFullName(person.FullName);

            var qoutaPerson = new QoutaPerson
            {
                QoutaAllocationId = model.QoutaAllocationId,
                FoodReceiverTypeId = 1,
                FoodTokenTypeId = model.FoodTokenTypeId,
                PersonId = person.Id,
                FName = fullName.FName,
                LName = fullName.LName,
                PersonalCode = person.PersonCode.Value.ToString(),
                PersonalTypeId = person.PersonTypeId,
                MainFoodId = model.MainFoodId,
                DessertId = model.DessertId,
                SideDishId = model.SideDishId,
                OrganGarrisonId = person.OrganGarrisonId,
                OrgId = allocation.OrgId,
                OrgTitle = allocation.OrganGarrison?.Title,
                DeliveryCode = GenerateUniqueDeliveryCode(),
                RegUserId = user.Id,
                CreateDate = DateTime.Now,
                IsDeleted = false
            };

            return base.Create(qoutaPerson);
        }
        catch
        {
            return new BaseResult(false, "ثبت غذا برای پرسنل با خطا همراه بوده است");
        }
    }

    public BaseResult CreateGuestFood(GuestFoodCreateDTO model)
    {
        try
        {
            var user = Session.GetUser();

            if (model == null || model.QoutaAllocationId <= 0 || model.Count <= 0 || model.MainFoodId <= 0)
                return new BaseResult(false, "اطلاعات ارسالی ناقص است");

            var allocation = UOW.QoutaAllocation
                .Get(x => x.Id == model.QoutaAllocationId && x.IsDeleted == false, null, null, null,
                    x => x.Include(i => i.OrganGarrison))
                .SingleOrDefault();

            if (allocation == null)
                return new BaseResult(false, "سهمیه مورد نظر یافت نشد");

            if (allocation.AllowOfficeUserRegister != true)
    return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

var accessResult = CheckAdministrativeManagerAccess(allocation);
if (!accessResult.Status)
    return accessResult;

            if (allocation.AllowOfficeUserRegister != true)
                return new BaseResult(false, "امکان ثبت غذا توسط کاربر اداری برای این سهمیه فعال نیست");

            if (allocation.RegisterDeadline.HasValue && DateTime.Now > allocation.RegisterDeadline.Value)
                return new BaseResult(false, "مهلت ثبت غذا برای این سهمیه به پایان رسیده است");

            var currentGuestCount = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                          x.FoodReceiverTypeId == 2 &&
                          x.IsDeleted == false)
                .Count();

            if (currentGuestCount + model.Count > allocation.GuestCapacity)
                return new BaseResult(false, $"ظرفیت مهمان تکمیل است. ظرفیت: {allocation.GuestCapacity}، ثبت شده: {currentGuestCount}");

            if (model.FoodTokenTypeId == 2)
            {
                var currentManagementCount = UOW.QoutaPerson
                    .Get(x => x.QoutaAllocationId == model.QoutaAllocationId &&
                              x.FoodTokenTypeId == 2 &&
                              x.IsDeleted == false)
                    .Count();

                if (currentManagementCount + model.Count > allocation.ManagementTokenCapacity)
                    return new BaseResult(false, $"ظرفیت ژتون مدیریتی تکمیل است. ظرفیت: {allocation.ManagementTokenCapacity}، ثبت شده: {currentManagementCount}");
            }

            for (int i = 1; i <= model.Count; i++)
            {
                var deliveryCode = GenerateUniqueDeliveryCode();

                var guest = new QoutaPerson
                {
                    QoutaAllocationId = model.QoutaAllocationId,
                    FoodReceiverTypeId = 2,
                    FoodTokenTypeId = model.FoodTokenTypeId,
                    PersonId = null,
                    FName = string.IsNullOrWhiteSpace(model.FName) ? "مهمان" : model.FName,
                    LName = string.IsNullOrWhiteSpace(model.LName) ? $"شماره {i}" : model.LName,
                    PersonalCode = $"G-{deliveryCode}",
                    PersonalTypeId = null,
                    MainFoodId = model.MainFoodId,
                    DessertId = model.DessertId,
                    SideDishId = model.SideDishId,
                    OrganGarrisonId = allocation.OrganGarrisonId,
                    OrgId = allocation.OrgId,
                    OrgTitle = allocation.OrganGarrison?.Title,
                    DeliveryCode = deliveryCode,
                    DeliveryHash = deliveryCode,
                    Description = model.Description,
                    RegUserId = user.Id,
                    CreateDate = DateTime.Now,
                    IsDeleted = false
                };

                var res = base.Create(guest);
                if (!res.Status)
                    return new BaseResult(false, res.Message);
            }

            return new BaseResult(true, $"{model.Count} ژتون مهمان با موفقیت ثبت شد");
        }
        catch
        {
            return new BaseResult(false, "ثبت غذای مهمان با خطا همراه بوده است");
        }
    }

    private BaseResult CheckCapacityForPerson(long qoutaAllocationId, long personalTypeId, long foodTokenTypeId)
    {
        var allocation = UOW.QoutaAllocation.GetById(qoutaAllocationId);

        if (allocation == null || allocation.IsDeleted == true)
            return new BaseResult(false, "سهمیه مورد نظر یافت نشد");

        var currentPersonTypeCount = UOW.QoutaPerson
            .Get(x => x.QoutaAllocationId == qoutaAllocationId &&
                      x.PersonalTypeId == personalTypeId &&
                      x.FoodReceiverTypeId == 1 &&
                      x.IsDeleted == false)
            .Count();

        if (personalTypeId == 1 && currentPersonTypeCount >= allocation.OfficerCapacity)
            return new BaseResult(false, "ظرفیت کادر تکمیل است");

        if (personalTypeId == 2 && currentPersonTypeCount >= allocation.SoldierCapacity)
            return new BaseResult(false, "ظرفیت وظیفه تکمیل است");

        if (foodTokenTypeId == 2)
        {
            var currentManagementCount = UOW.QoutaPerson
                .Get(x => x.QoutaAllocationId == qoutaAllocationId &&
                          x.FoodTokenTypeId == 2 &&
                          x.IsDeleted == false)
                .Count();

            if (currentManagementCount >= allocation.ManagementTokenCapacity)
                return new BaseResult(false, "ظرفیت ژتون مدیریتی تکمیل است");
        }

        return new BaseResult(true, "ظرفیت مجاز است");
    }

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
    public QoutaAllocationCapacityDTO GetCapacityStatus(long qoutaAllocationId)
    {
        var allocation = UOW.QoutaAllocation
            .Get(x => x.Id == qoutaAllocationId && x.IsDeleted == false, null, null, null,
                x => x.Include(i => i.OrganGarrison)
                      .Include(i => i.Day)
                      .Include(i => i.Meal))
            .SingleOrDefault();

        if (allocation == null)
            return null;

        var registeredItems = UOW.QoutaPerson
            .Get(x => x.QoutaAllocationId == qoutaAllocationId &&
                      x.IsDeleted == false)
            .ToList();

        var officerRegisteredCount = registeredItems
            .Count(x => x.FoodReceiverTypeId == 1 && x.PersonalTypeId == 1);

        var soldierRegisteredCount = registeredItems
            .Count(x => x.FoodReceiverTypeId == 1 && x.PersonalTypeId == 2);

        var guestRegisteredCount = registeredItems
            .Count(x => x.FoodReceiverTypeId == 2);

        var managementTokenRegisteredCount = registeredItems
            .Count(x => x.FoodTokenTypeId == 2);

        return new QoutaAllocationCapacityDTO
        {
            QoutaAllocationId = allocation.Id,

            OrganGarrisonTitle = allocation.OrganGarrison?.Title,
            QoutaAllocationDate = allocation.QoutaAllocationDate,

            DayTitle = allocation.Day?.Title ?? allocation.DayTitle,
            MealTitle = allocation.Meal?.Title ?? allocation.MealTitle,

            OfficerCapacity = allocation.OfficerCapacity,
            OfficerRegisteredCount = officerRegisteredCount,

            SoldierCapacity = allocation.SoldierCapacity,
            SoldierRegisteredCount = soldierRegisteredCount,

            GuestCapacity = allocation.GuestCapacity,
            GuestRegisteredCount = guestRegisteredCount,

            ManagementTokenCapacity = allocation.ManagementTokenCapacity,
            ManagementTokenRegisteredCount = managementTokenRegisteredCount,

            AllowOfficeUserRegister = allocation.AllowOfficeUserRegister == true,
            AllowPersonChange = allocation.AllowPersonChange == true,
            RegisterDeadline = allocation.RegisterDeadline
        };
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


    /// <summary>
    /// گرفتن لیست پرسنل یگان مربوط به سهمیه برای نمایش در فرم ثبت غذا
    /// </summary>
    public DataTableResponseDTO<QoutaPersonSelectableDTO> GetSelectablePersonsForQuota(
        DataTableSearchDTO searchData,
        long qoutaAllocationId)
    {
        var allocation = UOW.QoutaAllocation
            .Get(x => x.Id == qoutaAllocationId && x.IsDeleted == false)
            .SingleOrDefault();

        if (allocation == null || allocation.OrganGarrisonId == null)
        {
            return new DataTableResponseDTO<QoutaPersonSelectableDTO>
            {
                draw = searchData.draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<QoutaPersonSelectableDTO>()
            };
        }

        var persons = UOW.Person
            .Get(x => x.OrganGarrisonId == allocation.OrganGarrisonId.Value && x.IsDeleted == false, null, null, null,
                x => x.Include(i => i.PersonalType))
            .ToList();

        var qoutaPersons = UOW.QoutaPerson
            .Get(x => x.QoutaAllocationId == qoutaAllocationId && x.IsDeleted == false, null, null, null,
                x => x.Include(i => i.MainFood))
            .ToList();

        var query = persons
            .Select(person =>
            {
                var registered = qoutaPersons.FirstOrDefault(q => q.PersonId == person.Id);
                var fullName = SplitFullName(person.FullName);

                return new QoutaPersonSelectableDTO
                {
                    Id = person.Id,
                    PersonId = person.Id,

                    FName = fullName.FName,
                    LName = fullName.LName,

                    PersonalCode = person.PersonCode.HasValue
                        ? person.PersonCode.Value.ToString()
                        : person.NationalCode.HasValue
                            ? person.NationalCode.Value.ToString()
                            : "-",

                    PersonalTypeId = person.PersonTypeId,
                    PersonalTypeTitle = person.PersonalType != null ? person.PersonalType.Title : "-",

                    QoutaPersonId = registered?.Id,
                    FoodTitle = registered?.MainFood?.Title ?? "-",
                    IsRegistered = registered != null
                };
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(searchData.searchValue))
        {
            var searchValue = searchData.searchValue.Trim();

            query = query
                .Where(x =>
                    (x.FName != null && x.FName.Contains(searchValue)) ||
                    (x.LName != null && x.LName.Contains(searchValue)) ||
                    (x.PersonalCode != null && x.PersonalCode.Contains(searchValue)))
                .ToList();
        }

        var recordsTotal = query.Count;

        var data = query
            .Skip(searchData.start)
            .Take(searchData.length)
            .ToList();

        return new DataTableResponseDTO<QoutaPersonSelectableDTO>
        {
            draw = searchData.draw,
            recordsTotal = recordsTotal,
            recordsFiltered = recordsTotal,
            data = data
        };
    }

    #endregion
}