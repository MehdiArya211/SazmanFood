using Domain.Enums;

namespace DTO.Entities;

public class UnitStatisticViewDTO
{
    public long Id { get; set; }

    public int OrgId { get; set; }

    public string OrgTitle { get; set; }

    public int TotalOfficialCount { get; set; }

    public int TotalDutyCount { get; set; }

    public UnitStatisticStatus Status { get; set; }

    public string StatusTitle { get; set; }

    public string CreatorFullName { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateDateFa { get; set; }

    public string ApproverFullName { get; set; }

    public DateTime? ApproveDate { get; set; }

    public string ApproveDateFa { get; set; }

    public List<UnitStatisticDetailItemDTO>
        OfficialDetails
    { get; set; } = new();

    public List<UnitStatisticDetailItemDTO>
        DutyDetails
    { get; set; } = new();
}