using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;

namespace Food.Hubs.ZP;

/// <summary>
/// شناسه کیوسک را در هر درخواست از رکورد دستگاه در بانک API تازه می‌کند.
/// مقدار کوکی محافظت‌شده است و در صورت تغییر KioskId خودکار جایگزین می‌شود.
/// </summary>
public sealed class FaceKioskIdentity
{
    private const string CookieName = "Food.FaceKiosk";
    private readonly IDataProtector _protector;

    public FaceKioskIdentity(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("Food.FaceKiosk.Identity.v1");
    }

    public async Task<int> GetOrCreateAsync(HttpContext context, ILogger logger,
        KioskRegistryOptions registryOptions, long? requestedDeviceRecordId = null)
    {
        var connectionString = registryOptions.ConnectionString;
        var deviceRecordId = requestedDeviceRecordId ?? registryOptions.DeviceRecordId;
        if (string.IsNullOrWhiteSpace(connectionString) || deviceRecordId <= 0)
            throw new InvalidOperationException(
                "Configure ConnectionStrings:KioskRegistry and FaceLogin:DeviceRecordId.");

        int kioskId;
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(context.RequestAborted);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT KioskId FROM dbo.Devices WHERE Id = @Id";
            command.Parameters.Add("@Id", System.Data.SqlDbType.BigInt).Value = deviceRecordId;
            var result = await command.ExecuteScalarAsync(context.RequestAborted);
            if (result is null || result is DBNull ||
                !int.TryParse(Convert.ToString(result, CultureInfo.InvariantCulture), out kioskId) ||
                kioskId is < 100000 or > 999999)
                throw new InvalidOperationException(
                    $"Device record {deviceRecordId} is missing or has an invalid six-digit KioskId.");
        }

        if (TryGet(context, out var current) && current == kioskId)
            return kioskId;

        logger.LogInformation(
            "Kiosk ID refreshed from registry. DeviceRecordId: {DeviceRecordId}, KioskId: {KioskId}",
            deviceRecordId, kioskId);

        context.Response.Cookies.Append(CookieName,
            _protector.Protect(kioskId.ToString(CultureInfo.InvariantCulture)),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(2),
                MaxAge = TimeSpan.FromDays(730)
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
            if (int.TryParse(_protector.Unprotect(value), NumberStyles.None,
                    CultureInfo.InvariantCulture, out var parsedId) &&
                parsedId is >= 100000 and <= 999999)
            {
                kioskId = parsedId;
                return true;
            }

            return false;
        }
        catch (CryptographicException)
        {
            kioskId = 0;
            return false;
        }
    }
}
