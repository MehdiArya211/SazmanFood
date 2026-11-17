using BLL.Interface;
using Domain.Entities.FoodReservation;
using DTO.Base;
using DTO.Entities.FoodReservation;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ReserveManagment
{
    public interface IFoodReserveManager : IManager<FoodReserve, ApplicationContext>
    {
        public BaseResult AddToReserveAndReserveDetail(FoodReserveDTO model);

        Task<List<WeeklyFoodReserveDTO>> GetWeeklyFoodReserve(long userId);
        IReadOnlyDictionary<long, string> GetLegacyDeliveryQrImages(
            IReadOnlyCollection<long> detailIds, long userId, string siteBaseUrl);
        //Task PrintFoodReserveAsync(long userId);
        Task<BaseResult> PrintFoodReserveAsync(List<long> foodReserveDetaileIds, long UserId, string FullName);

        List<FoodReserveDetail> GetFoodReserveDetail(long userId);

        /// <summary>
        /// ثبت/ویرایش یک‌مرحله‌ای رزرو هفتگی بدون حذف رکوردهای قفل‌شده.
        /// editableDayIds روزهایی هستند که مهلت رزروشان هنوز نگذشته است.
        /// </summary>
        BaseResult SaveWeeklyReservation(
            FoodReserveDTO model,
            IReadOnlyCollection<(long DayId, long MealId)> editableSlots);

        /// <summary>حذف یک وعده رزروشده بدون دست‌زدن به وعده‌های دیگر.</summary>
        BaseResult DeleteMealReservation(long userId, DateTime weekStartDate, long dayId, long mealId);

    }
}
