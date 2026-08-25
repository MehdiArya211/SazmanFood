using System.Linq.Expressions;
using Utilities.Extentions;

namespace DTO.Entities.QoutaPerson
{
    public class MyFoodDataTableDTO
    {
        public long Id { get; set; }

        public DateTime QoutaAllocationDate { get; set; }

        public string QoutaAllocationDateFa => QoutaAllocationDate.ToPersianDateTime().ToString();

        public string DayTitle { get; set; }

        public string MealTitle { get; set; }

        public string FoodTitle { get; set; }

        public string FoodTokenTypeTitle { get; set; }

        public bool IsDelivered { get; set; }

        public bool CanChange { get; set; }

        public string DeliveryCode { get; set; }

        public static Expression<Func<Domain.Entities.QoutaPerson, MyFoodDataTableDTO>> Selector
        {
            get
            {
                return x => new MyFoodDataTableDTO
                {
                    Id = x.Id,
                    QoutaAllocationDate = x.QoutaAllocation.QoutaAllocationDate,
                    DayTitle = x.QoutaAllocation.DayTitle,
                    MealTitle = x.QoutaAllocation.MealTitle,
                    FoodTitle = x.MainFood != null ? x.MainFood.Title : "-",
                    FoodTokenTypeTitle = x.FoodTokenType != null ? x.FoodTokenType.Title : "-",
                    IsDelivered = x.IsDelivered,
                    DeliveryCode = x.DeliveryCode,

                    CanChange =
                        x.IsDelivered == false &&
                        x.QoutaAllocation.IsFinalized == false &&
                        x.QoutaAllocation.AllowPersonChange == true &&
                        (!x.QoutaAllocation.RegisterDeadline.HasValue ||
                         DateTime.Now <= x.QoutaAllocation.RegisterDeadline.Value)
                };
            }
        }
    }
}