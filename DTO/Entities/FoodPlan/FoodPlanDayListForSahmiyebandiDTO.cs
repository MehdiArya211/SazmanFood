using Domain.Entities;
using DTO.Entities;
using System.Linq.Expressions;

namespace DTO;

public class FoodPlanDayListForSahmiyebandiDTO
{
    public long Id { get; set; }
    public long FoodId { get; set; }
    public string FoodTitle { get; set; }


    /// <summary>
    /// فیلدهای لازم برای استخراج مدل از موجودیت مربوطه
    /// </summary>
    public static Expression<Func<FoodPlanDay, FoodPlanDayListForSahmiyebandiDTO>> Selector
    {
        get
        {
            return model => new FoodPlanDayListForSahmiyebandiDTO()
            {
                Id = model.Id,
                FoodId = model.FoodId,
                FoodTitle = model.Food.Title,


            };
        }
    }
}
