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
        //Task PrintFoodReserveAsync(long userId);
        Task<BaseResult> PrintFoodReserveAsync(List<long> foodReserveDetaileIds, long UserId, string FullName);

        List<FoodReserveDetail> GetFoodReserveDetail(long userId);

        /// <summary>
        /// ثبت/ویرایش یک‌مرحله‌ای رزرو هفتگی بدون حذف رکوردهای قفل‌شده.
        /// editableDayIds روزهایی هستند که مهلت رزروشان هنوز نگذشته است.
        /// </summary>
        BaseResult SaveWeeklyReservation(
            FoodReserveDTO model,
            IReadOnlyCollection<long> editableDayIds);

        /// <summary>حذف ایمن و یک‌مرحله‌ای رزرو یک روز از هفته.</summary>
        BaseResult DeleteDayReservation(long userId, DateTime weekStartDate, long dayId);

    }
}
