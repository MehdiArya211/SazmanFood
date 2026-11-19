using System;
using System.Collections.Generic;
using System.Linq;

namespace DTO.Base
{
    /// <summary>
    /// تنظیمات ساعت چاپ ژتون و مهلت رزرو هفتگی.
    /// </summary>
    public sealed class FoodTimingPolicyDTO
    {
        public bool PrintTimeEnabled { get; set; } = false;

        public string PrintStartTime { get; set; } = "12:00";

        public bool ReservationDeadlineEnabled { get; set; } = false;

        public bool BlockPastDays { get; set; } = false;
        public int MaxPrintCount { get; set; } = 3;

        public bool BreakfastReservable { get; set; } = true;
        public bool BreakfastVisible { get; set; } = true;
        public bool LunchReservable { get; set; } = true;
        public bool LunchVisible { get; set; } = true;
        public bool DinnerReservable { get; set; } = true;
        public bool DinnerVisible { get; set; } = true;
        public bool DessertReservable { get; set; } = true;
        public bool DessertVisible { get; set; } = true;
        public bool SideDishReservable { get; set; } = true;
        public bool SideDishVisible { get; set; } = true;

        public bool IsMealVisible(int code) => code switch
        {
            1 => BreakfastVisible,
            2 => LunchVisible,
            3 => DinnerVisible,
            _ => true
        };

        public bool IsMealReservable(int code) => code switch
        {
            1 => BreakfastReservable,
            2 => LunchReservable,
            3 => DinnerReservable,
            _ => true
        };

        public List<FoodReservationDeadlineDayDTO> Days { get; set; } =
            Enumerable.Range(0, 7)
                .Select(i => new FoodReservationDeadlineDayDTO
                {
                    DayOfWeek = i,
                    DaysBefore = 1,
                    CutoffTime = "12:00"
                }).ToList();
    }

    public sealed class FoodReservationDeadlineDayDTO
    {
        /// <summary>مقدار استاندارد DayOfWeek در دات نت: یکشنبه=0 تا شنبه=6</summary>
        public int DayOfWeek { get; set; }

        /// <summary>چند روز قبل از روز غذا امکان رزرو بسته می‌شود.</summary>
        public int DaysBefore { get; set; } = 1;

        /// <summary>ساعت نهایی در تاریخ مهلت (HH:mm).</summary>
        public string CutoffTime { get; set; } = "12:00";
    }
}
