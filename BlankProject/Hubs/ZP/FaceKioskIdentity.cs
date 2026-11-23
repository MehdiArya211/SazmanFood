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
        KioskRegistryOptions registryOptions)
    {
        var connectionString = registryOptions.ConnectionString;
        var clientIp = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(clientIp))
            throw new InvalidOperationException(
                "Kiosk registry connection or client IP is unavailable.");

        int kioskId;
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(context.RequestAborted);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT TOP (2) Id, KioskId FROM dbo.Devices WHERE LTRIM(RTRIM(Ip)) = @Ip";
            command.Parameters.Add("@Ip", System.Data.SqlDbType.NVarChar, 45).Value = clientIp;
            await using var reader = await command.ExecuteReaderAsync(context.RequestAborted);
            if (!await reader.ReadAsync(context.RequestAborted))
                throw new InvalidOperationException($"No kiosk is registered for client IP {clientIp}.");

            var deviceRecordId = reader.GetInt64(0);
            if (!int.TryParse(Convert.ToString(reader.GetValue(1), CultureInfo.InvariantCulture), out kioskId) ||
                kioskId is < 100000 or > 999999)
                throw new InvalidOperationException($"Device record {deviceRecordId} has an invalid six-digit KioskId.");
            if (await reader.ReadAsync(context.RequestAborted))
                throw new InvalidOperationException($"More than one kiosk is registered for client IP {clientIp}.");
        }

        if (TryGet(context, out var current) && current == kioskId)
            return kioskId;

        logger.LogInformation(
            "Kiosk ID refreshed from registry for client IP {ClientIp}. KioskId: {KioskId}",
            clientIp, kioskId);

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
