using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Base
{
    public sealed class PrinterOptions
    {
        public string FoodTokenPrinterName { get; set; } = "";
        public string FontName { get; set; } = "Tahoma";
        public int FontSize { get; set; } = 10;
        public int BoldFontSize { get; set; } = 11;
    }
}
