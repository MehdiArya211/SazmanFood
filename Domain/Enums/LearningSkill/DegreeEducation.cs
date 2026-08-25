using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum DegreeEducation 
    {
        [Description(" ابتدایی ")]
        Ebtedayi = 1,
        [Description("سیکل ")]
        Sikl = 2,
        [Description(" دیپلم ")]
        Diplum = 3,
             [Description(" کاردانی ")]
        Cardani = 4,
             [Description(" کارشناسی ")]
        Carshenasi = 5,
             [Description(" کارشناسی ارشد ")]
        CarshenasiArshad = 6,
             [Description(" دکترا ")]
        Doctora = 7,
    }
}
