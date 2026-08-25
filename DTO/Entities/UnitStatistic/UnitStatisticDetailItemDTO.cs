using Domain.Entities;
using System.Linq.Expressions;

namespace DTO.Entities
{
    public class UnitStatisticDetailItemDTO
    {
        public long? Id { get; set; }

        public long YeganTypeId { get; set; }

        public string YeganTypeTitle { get; set; }

        public int Count { get; set; }

        public static Expression<
            Func<UnitStatisticDetail, UnitStatisticDetailItemDTO>>
            Selector
        {
            get
            {
                return model => new UnitStatisticDetailItemDTO
                {
                    Id = model.Id,

                    YeganTypeId =
                        model.YeganTypeId,

                    YeganTypeTitle =
                        model.YeganType.Title,

                    Count =
                        model.Count
                };
            }
        }
    }
}