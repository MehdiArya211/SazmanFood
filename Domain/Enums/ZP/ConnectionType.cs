using System.ComponentModel;

namespace Domain.Enums.ZP
{
    public enum ConnectionType
    {
        [Description("شبکه")]
        Network = 0,

        [Description("یو اس بی")]
        Usb = 1
    }
}
