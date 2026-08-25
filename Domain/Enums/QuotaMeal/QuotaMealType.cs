using System.ComponentModel;

namespace Domain.Enums;

public enum QuotaMealType
{
    [Description("صبحانه")]
    Breakfast = 1,

    [Description("ناهار")]
    Lunch = 2,

    [Description("شام")]
    Dinner = 3
}