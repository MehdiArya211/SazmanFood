using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace Food.Hubs.ZP;

/// <summary>
/// یک شناسه پایدار برای هر مرورگر کیوسک صادر می‌کند. مقدار کوکی امضا و رمزگذاری
/// می‌شود تا مرورگر نتواند شناسه کیوسک دیگری را به سرور معرفی کند.
/// </summary>
public sealed class FaceKioskIdentity
{
    private const string CookieName = "Food.FaceKiosk";
    private readonly IDataProtector _protector;

    public FaceKioskIdentity(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("Food.FaceKiosk.Identity.v1");
    }

    public int GetOrCreate(HttpContext context)
    {
        if (TryGet(context, out var kioskId))
            return kioskId;

        kioskId = RandomNumberGenerator.GetInt32(1, int.MaxValue);
        context.Response.Cookies.Append(CookieName,
            _protector.Protect(kioskId.ToString(CultureInfo.InvariantCulture)),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(2)
            });
        return kioskId;
    }

    public bool TryGet(HttpContext context, out int kioskId)
    {
        kioskId = 0;
        if (!context.Request.Cookies.TryGetValue(CookieName, out var value) ||
            string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            return int.TryParse(_protector.Unprotect(value),
                       NumberStyles.None, CultureInfo.InvariantCulture, out kioskId) &&
                   kioskId > 0;
        }
        catch (CryptographicException)
        {
            kioskId = 0;
            return false;
        }
    }
}
