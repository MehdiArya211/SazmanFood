using Domain.Enums.ZP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Printer
{
    [Key]
    [Display(Name = "شناسه")]
    public long Id { get; set; }
    public string Title { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public string Ip { get; set; }
    public int Port { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}
