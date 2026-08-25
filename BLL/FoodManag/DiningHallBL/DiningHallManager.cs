using Domain.Entities.FoodManage;
using Domain.Enums.Food;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.DiningHalDTo;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Services.RedisService;
using Utilities.Extentions;

namespace BLL.FoodManag.DiningHallBL
{
    public class DiningHallManager
        : Manager<DiningHall, ApplicationContext>,
          IDiningHallManager
    {
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly ISession Session;
        private readonly IRedisManager Redis;

        public DiningHallManager(
            DbContexts context,
            IHttpContextAccessor httpContextAccessor,
            IRedisManager redis)
            : base(context, httpContextAccessor)
        {
            this.httpContextAccessor =
                httpContextAccessor ??
                throw new ArgumentNullException(
                    nameof(httpContextAccessor));

            Redis =
                redis ??
                throw new ArgumentNullException(
                    nameof(redis));

            // اگر HttpContext وجود نداشت، Session برابر null می‌ماند.
            Session =
                httpContextAccessor.HttpContext?.Session;
        }

        /// <summary>
        /// گرفتن لیست سالن‌های غذاخوری برای DataTable
        /// </summary>
        public DataTableResponseDTO<DiningHallDTO>
            GetDataTableDTO(
                DataTableSearchDTO searchData,
                DiningHallFilterDTO filters)
        {
            return UOW.diningHall.GetDataTableDTO(
                searchData,
                filters);
        }

        /// <summary>
        /// ایجاد سالن غذاخوری جدید
        /// </summary>
        public BaseResult CreateDiningHalls(
            DiningHallCreateDTO model)
        {
            if (model == null)
            {
                return new BaseResult(
                    false,
                    "اطلاعات ارسالی معتبر نیست.");
            }

            #region آماده‌سازی اطلاعات

            model.Title = model.Title?
                .Trim()
                .ToPersianCharacter();

            model.OrgTitle = model.OrgTitle?
                .Trim()
                .ToPersianCharacter();

            model.ManagerName = model.ManagerName?
                .Trim()
                .ToPersianCharacter();

            model.PhoneNumber = model.PhoneNumber?
                .Trim()
                .ToEnglishNumber();

            model.Description = model.Description?
                .Trim()
                .ToPersianCharacter();

            #endregion

            #region اعتبارسنجی نام سالن

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return new BaseResult(
                    false,
                    "نام سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی یگان

            if (model.OrgId <= 0)
            {
                return new BaseResult(
                    false,
                    "یگان سالن غذاخوری را مشخص کنید.");
            }

            #endregion

            #region اعتبارسنجی مسئول سالن

            if (string.IsNullOrWhiteSpace(
                    model.ManagerName))
            {
                return new BaseResult(
                    false,
                    "نام مسئول سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی شماره تماس

            if (string.IsNullOrWhiteSpace(
                    model.PhoneNumber))
            {
                return new BaseResult(
                    false,
                    "شماره تماس سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی نوع پرسنل

            if (model.PersonalTypeId <= 0)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل استفاده‌کننده را مشخص کنید.");
            }

            var personalType =
                UOW.PersonalType.FirstOrDefault(x =>
                    x.Id == model.PersonalTypeId);

            if (personalType == null)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل انتخاب‌شده معتبر نیست.");
            }

            #endregion

            #region اعتبارسنجی نوع استفاده

            if (!Enum.IsDefined(
                    typeof(DiningHallUsageType),
                    model.UsageType))
            {
                return new BaseResult(
                    false,
                    "نوع استفاده از سالن معتبر نیست.");
            }

            #endregion

            #region اعتبارسنجی محل تهیه غذا

            //if (!Enum.IsDefined(
            //        typeof(DiningHallSupplyType),
            //        model.SupplyType))
            //{
            //    return new BaseResult(
            //        false,
            //        "محل تهیه غذا معتبر نیست.");
            //}

            #endregion

            #region اعتبارسنجی ظرفیت سالن

            if (model.HallCapacity.HasValue &&
                model.HallCapacity.Value <= 0)
            {
                return new BaseResult(
                    false,
                    "ظرفیت سالن باید بیشتر از صفر باشد.");
            }

            #endregion

            #region بررسی تکراری بودن نام سالن در یگان

            var duplicateDiningHall =
                UOW.diningHall.FirstOrDefault(x =>
                    x.OrgId == model.OrgId &&
                    x.Title == model.Title);

            if (duplicateDiningHall != null)
            {
                return new BaseResult(
                    false,
                    "سالن غذاخوری با این نام قبلاً برای یگان انتخاب‌شده ثبت شده است.");
            }

            #endregion

            #region کنترل سالن داخلی یگان

            if (model.SupplyType ==
                DiningHallSupplyType.InternalOrganization)
            {
                if (!model.KitchenId.HasValue ||
                    model.KitchenId.Value <= 0)
                {
                    return new BaseResult(
                        false,
                        "برای سالن داخلی، انتخاب آشپزخانه الزامی است.");
                }

                var internalDiningHall =
                    UOW.diningHall.FirstOrDefault(x =>
                        x.OrgId == model.OrgId &&
                        x.SupplyType ==
                        DiningHallSupplyType
                            .InternalOrganization);

                if (internalDiningHall != null)
                {
                    return new BaseResult(
                        false,
                        "برای این یگان قبلاً یک سالن غذاخوری داخلی تعریف شده است.");
                }

                var kitchen =
                    UOW.kitchen.FirstOrDefault(x =>
                        x.Id == model.KitchenId.Value);

                if (kitchen == null)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده یافت نشد.");
                }

                if (!kitchen.IsActive)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده غیرفعال است.");
                }

                if (kitchen.OrgId != model.OrgId)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده متعلق به یگان سالن نیست.");
                }
            }
            else
            {
                // برای سالن‌هایی که غذای داخلی ندارند
                // ارتباط مستقیم با آشپزخانه داخلی پاک می‌شود.
                model.KitchenId = null;
            }

            #endregion

            var diningHall = new DiningHall()
            {
                Title = model.Title,

                OrgId = model.OrgId,
                OrgTitle = model.OrgTitle,

                ManagerName = model.ManagerName,
                PhoneNumber = model.PhoneNumber,

                PersonalTypeId =
                    model.PersonalTypeId,

                UsageType =
                    model.UsageType,

                SupplyType =
                    model.SupplyType,

                KitchenId =
                    model.KitchenId,

                HallCapacity =
                    model.HallCapacity,

                Description =
                    model.Description,

                IsActive =
                    model.IsActive
            };

            return base.Create(diningHall);
        }

        /// <summary>
        /// گرفتن مدل لازم برای ویرایش سالن غذاخوری
        /// </summary>
        public DiningHallEditDTO
            GetDiningHallForEditDTO(long? id)
        {
            if (id == null || id <= 0)
            {
                return null;
            }

            return UOW.diningHall
                .GetOneDTO<DiningHallEditDTO>(
                    DiningHallEditDTO.Selector,
                    x => x.Id == id);
        }

        /// <summary>
        /// ویرایش سالن غذاخوری
        /// </summary>
        public BaseResult UpdateDiningHall(
            DiningHallEditDTO model)
        {
            if (model == null || model.Id <= 0)
            {
                return new BaseResult(
                    false,
                    "اطلاعات سالن غذاخوری معتبر نیست.");
            }

            var diningHall =
                UOW.diningHall.FirstOrDefault(x =>
                    x.Id == model.Id);

            if (diningHall == null)
            {
                return new BaseResult(
                    false,
                    "سالن غذاخوری مورد نظر یافت نشد.");
            }

            #region آماده‌سازی اطلاعات

            model.Title = model.Title?
                .Trim()
                .ToPersianCharacter();

            model.OrgTitle = model.OrgTitle?
                .Trim()
                .ToPersianCharacter();

            model.ManagerName = model.ManagerName?
                .Trim()
                .ToPersianCharacter();

            model.PhoneNumber = model.PhoneNumber?
                .Trim()
                .ToEnglishNumber();

            model.Description = model.Description?
                .Trim()
                .ToPersianCharacter();

            #endregion

            #region اعتبارسنجی نام سالن

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return new BaseResult(
                    false,
                    "نام سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی یگان

            if (model.OrgId <= 0)
            {
                return new BaseResult(
                    false,
                    "یگان سالن غذاخوری را مشخص کنید.");
            }

            #endregion

            #region اعتبارسنجی مسئول سالن

            if (string.IsNullOrWhiteSpace(
                    model.ManagerName))
            {
                return new BaseResult(
                    false,
                    "نام مسئول سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی شماره تماس

            if (string.IsNullOrWhiteSpace(
                    model.PhoneNumber))
            {
                return new BaseResult(
                    false,
                    "شماره تماس سالن غذاخوری الزامی است.");
            }

            #endregion

            #region اعتبارسنجی نوع پرسنل

            if (model.PersonalTypeId <= 0)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل استفاده‌کننده را مشخص کنید.");
            }

            var personalType =
                UOW.PersonalType.FirstOrDefault(x =>
                    x.Id == model.PersonalTypeId);

            if (personalType == null)
            {
                return new BaseResult(
                    false,
                    "نوع پرسنل انتخاب‌شده معتبر نیست.");
            }

            #endregion

            #region اعتبارسنجی نوع استفاده

            if (!Enum.IsDefined(
                    typeof(DiningHallUsageType),
                    model.UsageType))
            {
                return new BaseResult(
                    false,
                    "نوع استفاده از سالن معتبر نیست.");
            }

            #endregion

            #region اعتبارسنجی ظرفیت

            if (model.HallCapacity.HasValue &&
                model.HallCapacity.Value <= 0)
            {
                return new BaseResult(
                    false,
                    "ظرفیت سالن باید بیشتر از صفر باشد.");
            }

            #endregion

            #region بررسی تکراری بودن نام سالن در یگان

            var duplicateDiningHall =
                UOW.diningHall.FirstOrDefault(x =>
                    x.Id != model.Id &&
                    x.OrgId == model.OrgId &&
                    x.Title == model.Title);

            if (duplicateDiningHall != null)
            {
                return new BaseResult(
                    false,
                    "سالن غذاخوری با این نام قبلاً برای یگان انتخاب‌شده ثبت شده است.");
            }

            #endregion

            #region کنترل سالن داخلی یگان

            if (model.SupplyType ==
                DiningHallSupplyType.InternalOrganization)
            {
                if (!model.KitchenId.HasValue ||
                    model.KitchenId.Value <= 0)
                {
                    return new BaseResult(
                        false,
                        "برای سالن داخلی، انتخاب آشپزخانه الزامی است.");
                }

                var internalDiningHall =
                    UOW.diningHall.FirstOrDefault(x =>
                        x.Id != model.Id &&
                        x.OrgId == model.OrgId &&
                        x.SupplyType ==
                        DiningHallSupplyType
                            .InternalOrganization);

                if (internalDiningHall != null)
                {
                    return new BaseResult(
                        false,
                        "برای این یگان قبلاً یک سالن غذاخوری داخلی تعریف شده است.");
                }

                var kitchen =
                    UOW.kitchen.FirstOrDefault(x =>
                        x.Id == model.KitchenId.Value);

                if (kitchen == null)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده یافت نشد.");
                }

                if (!kitchen.IsActive)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده غیرفعال است.");
                }

                if (kitchen.OrgId != model.OrgId)
                {
                    return new BaseResult(
                        false,
                        "آشپزخانه انتخاب‌شده متعلق به یگان سالن نیست.");
                }
            }
            else
            {
                model.KitchenId = null;
            }

            #endregion

            diningHall.Title =
                model.Title;

            diningHall.OrgId =
                model.OrgId;

            diningHall.OrgTitle =
                model.OrgTitle;

            diningHall.ManagerName =
                model.ManagerName;

            diningHall.PhoneNumber =
                model.PhoneNumber;

            diningHall.PersonalTypeId =
                model.PersonalTypeId;

            diningHall.UsageType =
                model.UsageType;

            diningHall.SupplyType =
                model.SupplyType;

            diningHall.KitchenId =
                model.KitchenId;

            diningHall.HallCapacity =
                model.HallCapacity;

            diningHall.Description =
                model.Description;

            diningHall.IsActive =
                model.IsActive;

            return base.Update(diningHall);
        }

        /// <summary>
        /// گرفتن تعداد کل سالن‌های غذاخوری
        /// </summary>
        public int GetTotalCount()
        {
            return UOW.diningHall.Count();
        }

        /// <summary>
        /// بررسی تکراری نبودن نام سالن در یک یگان
        /// </summary>
        public bool TitleIsUnique(
            string title,
            int orgId,
            long? id = null)
        {
            if (string.IsNullOrWhiteSpace(title) ||
                orgId <= 0)
            {
                return false;
            }

            title = title
                .Trim()
                .ToPersianCharacter();

            var diningHall =
                UOW.diningHall.FirstOrDefault(x =>
                    x.Title == title &&
                    x.OrgId == orgId &&
                    (!id.HasValue || x.Id != id.Value));

            return diningHall == null;
        }

        /// <summary>
        /// بررسی وجود سالن داخلی برای یگان
        /// </summary>
        public bool InternalDiningHallExists(
            int orgId,
            long? id = null)
        {
            if (orgId <= 0)
            {
                return false;
            }

            var diningHall =
                UOW.diningHall.FirstOrDefault(x =>
                    x.OrgId == orgId &&
                    x.SupplyType ==
                    DiningHallSupplyType
                        .InternalOrganization &&
                    (!id.HasValue || x.Id != id.Value));

            return diningHall != null;
        }

        /// <summary>
        /// حذف سالن غذاخوری
        /// </summary>
        public override bool Delete(object id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }

                var diningHall =
                    UOW.diningHall.GetById(id);

                if (diningHall == null)
                {
                    return false;
                }

                UOW.diningHall.Remove(
                    diningHall);

                return UOW.Commit();
            }
            catch
            {
                return false;
            }
        }
    }
}