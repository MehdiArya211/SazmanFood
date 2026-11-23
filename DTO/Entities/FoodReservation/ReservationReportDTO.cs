using System;
using System.Collections.Generic;
using System.Linq;

namespace DTO.Entities.FoodReservation
{
    public class ReservationReportRowDTO
    {
        public string Period { get; set; } = "";
        public string Meal { get; set; } = "";
        public string Food { get; set; } = "";
        public int Official { get; set; }
        public int Duty { get; set; }
        public int Unknown { get; set; }
        public int Total => Official + Duty + Unknown;
    }

    public class ReservationReportDTO
    {
        public string Title { get; set; } = "";
        public string Kind { get; set; } = "";
        public string GroupBy { get; set; } = "daily";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? MealId { get; set; }
        public long? FoodId { get; set; }
        public List<ReservationReportRowDTO> Rows { get; set; } = new();
        public List<ReservationReportOptionDTO> Meals { get; set; } = new();
        public List<ReservationReportOptionDTO> Foods { get; set; } = new();
        public int Total => Rows.Sum(x => x.Total);
        public int Official => Rows.Sum(x => x.Official);
        public int Duty => Rows.Sum(x => x.Duty);
        public int Unknown => Rows.Sum(x => x.Unknown);
    }

    public class ReservationReportOptionDTO
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
    }
}
