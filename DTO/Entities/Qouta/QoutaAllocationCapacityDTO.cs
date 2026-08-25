namespace DTO.Entities;

/// <summary>
/// وضعیت ظرفیت سهمیه
/// </summary>
public class QoutaAllocationCapacityDTO
{
    public long QoutaAllocationId { get; set; }

    public string OrganGarrisonTitle { get; set; }

    public DateTime QoutaAllocationDate { get; set; }

    public string DayTitle { get; set; }

    public string MealTitle { get; set; }

    public int OfficerCapacity { get; set; }
    public int OfficerRegisteredCount { get; set; }
    public int OfficerRemainingCount => OfficerCapacity - OfficerRegisteredCount;

    public int SoldierCapacity { get; set; }
    public int SoldierRegisteredCount { get; set; }
    public int SoldierRemainingCount => SoldierCapacity - SoldierRegisteredCount;

    public int GuestCapacity { get; set; }
    public int GuestRegisteredCount { get; set; }
    public int GuestRemainingCount => GuestCapacity - GuestRegisteredCount;

    public int ManagementTokenCapacity { get; set; }
    public int ManagementTokenRegisteredCount { get; set; }
    public int ManagementTokenRemainingCount => ManagementTokenCapacity - ManagementTokenRegisteredCount;

    public bool AllowOfficeUserRegister { get; set; }
    public bool AllowPersonChange { get; set; }
    public DateTime? RegisterDeadline { get; set; }
}