using System.ComponentModel.DataAnnotations;
namespace Domain.Entities;

public class Devices
{
    [Key]
    [Display(Name = "شناسه")]
    public long Id { get; set; }
    public int KioskId { get; set; }
    public long PrinterId { get; set; }
    public string SerialNumber { get; set; }
    #region Relation
    public Printer Printer { get; set; }
    #endregion
}
