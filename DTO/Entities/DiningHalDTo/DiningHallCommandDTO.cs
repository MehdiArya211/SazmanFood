using Domain.Enums.Food;
using System.ComponentModel.DataAnnotations;

namespace DTO.Entities;

public abstract class DiningHallCommandDTO
{
    public string Title { get; set; }

    public int OrgId { get; set; }

    public string OrgTitle { get; set; }

    public string ManagerName { get; set; }

    public string PhoneNumber { get; set; }

    public long PersonalTypeId { get; set; }

    public DiningHallUsageType UsageType { get; set; }

    public DiningHallSupplyType SupplyType { get; set; }

    public long? KitchenId { get; set; }

    public int? HallCapacity { get; set; }

    public string HallCapacitySep { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}
