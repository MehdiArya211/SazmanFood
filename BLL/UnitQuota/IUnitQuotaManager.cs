using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.Entities;
using Infrastructure.Data;

namespace BLL.Interface
{
    /// <summary>
    /// مدیریت نمایش سهمیه و ثبت نفرات کادر و وظیفه
    /// </summary>
    public interface IUnitQuotaManager :
        IManager<UnitQuota, ApplicationContext>
    {
        /// <summary>
        /// دریافت اطلاعات مودال ثبت کارکنان کادر
        /// </summary>
        /// <param name="unitQuotaId">شناسه سهمیه</param>
        /// <param name="mealId">شناسه وعده</param>
        UnitQuotaPersonFormDTO GetOfficialForm(
            long unitQuotaId,
            long mealId);

        /// <summary>
        /// دریافت اطلاعات مودال ثبت کارکنان وظیفه
        /// </summary>
        /// <param name="unitQuotaId">شناسه سهمیه</param>
        /// <param name="mealId">شناسه وعده</param>
        UnitQuotaPersonFormDTO GetDutyForm(
            long unitQuotaId,
            long mealId);

        /// <summary>
        /// ثبت کارکنان کادر در سهمیه
        /// </summary>
        /// <param name="model">اطلاعات فرم ثبت کادر</param>
        /// <param name="person">
        /// اطلاعات واقعی پرسنل دریافت‌شده از وب‌سرویس
        /// </param>
        BaseResult CreateOfficial(
            UnitQuotaOfficialCreateDTO model,
            PersonalInfDTO person);

        /// <summary>
        /// ثبت کارکنان وظیفه در سهمیه
        /// </summary>
        /// <param name="model">اطلاعات فرم ثبت وظیفه</param>
        BaseResult CreateDuty(
            UnitQuotaDutyCreateDTO model);

        /// <summary>
        /// دریافت نفرات ثبت‌شده یک سهمیه و وعده
        /// </summary>
        /// <param name="unitQuotaId">شناسه سهمیه</param>
        /// <param name="mealId">شناسه وعده</param>
        List<UnitQuotaPersonDTO> GetPersons(
            long unitQuotaId,
            long mealId);

        /// <summary>
        /// دریافت سالن‌های فعال یگان متناسب با نوع پرسنل
        /// </summary>
        /// <param name="orgId">شناسه یگان</param>
        /// <param name="personalTypeId">شناسه نوع پرسنل</param>
        IList<SelectListDTO> GetDiningHalls(
            int orgId,
            long personalTypeId);

        /// <summary>
        /// حذف فرد ثبت‌شده از سهمیه
        /// </summary>
        /// <param name="id">شناسه رکورد UnitQuotaPerson</param>
        BaseResult DeletePerson(long id);

        /// <summary>
        /// دریافت لیست تجمیع‌شده سهمیه‌ها
        /// </summary>
        List<UnitQuotaDTO> GetList(
            UnitQuotaFilterDTO filters,
            bool canViewAllOrganizations);

        /// <summary>
        /// تاریخ و وعده‌های قابل رزرو کاربر را بر اساس سهمیه جدید یگان برمی‌گرداند.
        /// </summary>
        IReadOnlyCollection<(DateTime Date, long MealId)> GetReservableSlots(
            long? personId,
            string personCode,
            string nationalCode,
            int orgId,
            DateTime fromDate,
            DateTime toDate);
    }
}