using System;
using System.Collections.Generic;

namespace DTO.Base
{
    /// <summary>
    /// نتیجه بررسی فقط-خواندنی چاپگر از حساب واقعی فرآیند وب.
    /// این داده‌ها هرگز خروج فیزیکی کاغذ را تأیید نمی‌کنند.
    /// </summary>
    public sealed class PrinterDiagnosticsDTO
    {
        public string ServerName { get; set; } = string.Empty;
        public string ProcessUser { get; set; } = string.Empty;
        public string ConfiguredPrinterName { get; set; } = string.Empty;
        public bool IsWindows { get; set; }
        public bool PrinterConfigured { get; set; }
        public bool PrinterVisible { get; set; }
        public bool PrinterValid { get; set; }
        public string PaperName { get; set; } = string.Empty;
        public int? PaperWidthHundredthsInch { get; set; }
        public int? PaperHeightHundredthsInch { get; set; }
        public List<string> InstalledPrinters { get; set; } = new();
        public string Error { get; set; } = string.Empty;
    }
}
