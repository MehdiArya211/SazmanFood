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
    /// گرفتن لیست پرسنل براساس سهمیه یگان
    /// </summary>
    /// <param name="qoutaAllocationId">شناسه سهمیه بندی</param>
    /// <returns></returns>
    List<QoutaPersonDataTableDTO> GetPersonListWithQouataAllocation(long qoutaAllocationId);


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
    /// ثبت یک‌بارمصرف تحویل غذا با اطلاعات امن QR.
    /// </summary>
    BaseResult DeliverFoodByQr(string deliveryCode, string deliveryHash, long mealId);


    /// <summary>
    /// گرفتن لیست غذای ثبت شده برای پرسنل لاگین شده
    /// </summary>
    /// <param name="searchData">اطلاعات دیتاتیبل</param>
    /// <param name="personId">شناسه پرسنل</param>
    /// <returns></returns>
    DataTableResponseDTO<MyFoodDataTableDTO> GetMyFoodDataTableDTO(DataTableSearchDTO searchData, long personId);

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