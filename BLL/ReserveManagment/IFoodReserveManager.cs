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
        BaseResult ValidateQuotaForReservation(FoodReserveDTO model);

        Task<List<WeeklyFoodReserveDTO>> GetWeeklyFoodReserve(long userId);
        //Task PrintFoodReserveAsync(long userId);
        Task PrintFoodReserveAsync(List<long> foodReserveDetaileIds, long UserId, string FullName);

        List<FoodReserveDetail> GetFoodReserveDetail(long userId);

    }
}
