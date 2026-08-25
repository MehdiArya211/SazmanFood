using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Enums
{
    public enum AcceptanceStat
    {
        
        [Display(Name = "قبول")]
        [Description("قبول")] 
        Ghabol = 1,

        [Description("مردود")]
        [Display(Name = "مردود")]
        Mardod =2,

        [Description("انصراف")]
        [Display(Name = "انصراف")]
        Enseraf =3,





    }


}
