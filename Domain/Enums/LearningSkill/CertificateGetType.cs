using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Enums
{
    public enum CertificateGetType
    {
        [Display(Name = "قبل خدمت")]
        ghabl = 1,


        [Display(Name = "حین خدمت ")]
        hean = 2,


        [Display(Name = "به صورت کلاسیک")]
        classic = 3

    }
}
