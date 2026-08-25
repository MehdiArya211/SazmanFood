using Domain.Enums;
using Utilities.Extentions;

namespace DTO.Entities
{
    public class UnitCalendarDTO
    {
        public long Id { get; set; }

        public int OrgId { get; set; }

        public string OrgTitle { get; set; }

        public DateTime CalendarDate { get; set; }

        public DayType DayType { get; set; }

        public string DayTypeTitle
        {
            get
            {
                return DayType switch
                {
                    DayType.Normal => "عادی",
                    DayType.Holiday => "تعطیل",
                    DayType.HalfHoliday => "نیمه تعطیل",
                    _ => "نامشخص"
                };
            }
        }

        public string CalendarDateFa
        {
            get
            {
                return CalendarDate
                    .ToPersianDateTime()
                    .ToString();
            }
        }

        public string Description { get; set; }

        public DateTime? RegDate { get; set; }

        public string RegDateFa
        {
            get
            {
                return RegDate.HasValue
                    ? RegDate.Value
                        .ToPersianDateTime()
                        .ToString()
                    : "-";
            }
        }
    }
}