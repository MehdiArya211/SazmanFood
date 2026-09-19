using BLL.Interface;
using Domain.Entities;
using DTO;
using DTO.Base;
using DTO.DataTable;
using DTO.Entities;
using DTO.Entities.QoutaPerson;
using Infrastructure.Data;

namespace BLL;

/// <summary>
/// مدیریت نفرات تخصیص یافته به سهمیه غذا
/// </summary>
public interface IQoutaPersonManager : IManager<QoutaPerson, ApplicationContext>
{
    /// <summary>
    /// دریافت لیست نفرات سهمیه برای نمایش در DataTable
    /// </summary>
    /// <param name="searchData">اطلاعات جستجو و صفحه‌بندی</param>
    /// <param name="id">شناسه سهمیه غذا</param>
    /// <returns></returns>
    DataTableResponseDTO<QoutaPersonDataTableDTO> GetDataTableDTO(DataTableSearchDTO searchData, long id);

    /// <summary>
    /// ثبت نفر سهمیه
    /// </summary>
    /// <param name="model">مدل ثبت نفر سهمیه</param>
    /// <returns></returns>
    BaseResult Create(QoutaPersonCreateDTO model);

    /// <summary>
    /// گرفتن سهمیه مجاز کادر و وظیفه
    /// </summary>
    /// <param name="qoutaAllocationId">شناسه سهمیه بندی</param>
    /// <returns></returns>
    OrganSahmiyeDTO GetSahmiye(long qoutaAllocationId);

    /// <summary>
    /// گرفتن سهمیه مجاز کادر و وظیفه براساس نوع یگان
    /// </summary>
    /// <param name="qoutaAllocationId">شناسه سهمیه بندی</param>
    /// <returns></returns>
    OrganSahmiyeDTO GetSahmiyeAi(long qoutaAllocationId);

    /// <summary>
    /// گرفتن لیست پرسنل براساس سهمیه یگان
    /// </summary>
    /// <param name="qoutaAllocationId">شناسه سهمیه بندی</param>
    /// <returns></returns>
    List<QoutaPersonDataTableDTO> GetPersonListWithQouataAllocation(long qoutaAllocationId);

    /// <summary>
    /// ثبت تکی غذا برای پرسنل
    /// </summary>
    /// <param name="model">مدل ثبت تکی</param>
    /// <returns></returns>
    BaseResult CreateSingle(QoutaPersonCreateDTO model);

    /// <summary>
    /// ثبت گروهی غذا برای پرسنل
    /// </summary>
    /// <param name="model">مدل ثبت گروهی</param>
    /// <returns></returns>
    BaseResult CreateBulk(QoutaPersonBulkCreateDTO model);

    /// <summary>
    /// ثبت غذا برای پرسنل توسط کاربر اداری قسمت
    /// </summary>
    /// <param name="model">مدل ثبت غذا توسط اداری</param>
    /// <returns></returns>
    BaseResult CreateForOfficeUser(QoutaPersonOfficeCreateDTO model);

    /// <summary>
    /// ثبت غذا برای مهمان توسط کاربر اداری قسمت
    /// </summary>
    /// <param name="model">مدل ثبت غذای مهمان</param>
    /// <returns></returns>
    BaseResult CreateGuestFood(GuestFoodCreateDTO model);

    /// <summary>
    /// تغییر غذای ثبت شده توسط پرسنل
    /// </summary>
    /// <param name="model">مدل تغییر غذا</param>
    /// <returns></returns>
    BaseResult ChangePersonFood(ChangePersonFoodDTO model);

    /// <summary>
    /// تحویل غذا با کد ژتون یا QR
    /// </summary>
    /// <param name="model">مدل تحویل غذا با کد</param>
    /// <returns></returns>
    BaseResult DeliverFoodByCode(DeliverFoodByCodeDTO model);

    /// <summary>
    /// دریافت وضعیت ظرفیت، ثبت شده و باقی مانده سهمیه
    /// </summary>
    /// <param name="qoutaAllocationId">شناسه سهمیه بندی</param>
    /// <returns></returns>
    QoutaAllocationCapacityDTO GetCapacityStatus(long qoutaAllocationId);


    /// <summary>
    /// گرفتن لیست غذای ثبت شده برای پرسنل لاگین شده
    /// </summary>
    /// <param name="searchData">اطلاعات دیتاتیبل</param>
    /// <param name="personId">شناسه پرسنل</param>
    /// <returns></returns>
    DataTableResponseDTO<MyFoodDataTableDTO> GetMyFoodDataTableDTO(DataTableSearchDTO searchData, long personId);

    /// <summary>
    /// گرفتن پرسنل یگان برای ثبت غذا در سهمیه
    /// </summary>
    DataTableResponseDTO<QoutaPersonSelectableDTO> GetSelectablePersonsForQuota(
        DataTableSearchDTO searchData,
        long qoutaAllocationId);

    /// <summary>
    /// بررسی وجود سهمیه فعال برای کد پرسنلی.
    /// </summary>
    bool HasActiveQuota(string personalCode);


    /// <summary>
    /// دریافت تاریخ و وعده‌هایی که برای کد پرسنلی سهمیه ثبت شده است.
    /// </summary>
    IReadOnlyCollection<(DateTime Date, long MealId)> GetReservableSlots(
        string personalCode,
        DateTime fromDate,
        DateTime toDate);

}