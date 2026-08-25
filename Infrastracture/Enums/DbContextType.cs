using System.ComponentModel;

namespace Infrastructure.Enums
{
    /// <summary>
    /// نوع کانتکس
    /// </summary>
    public enum DbContextType
    {
        [Description("کانتکس اصلی")]
        ApplicationContext = 0,

        [Description("کانتکس لاگ گیری")]
        LogContext = 1,
    }
}
