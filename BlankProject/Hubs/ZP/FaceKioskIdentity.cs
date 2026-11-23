using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;

namespace Food.Hubs.ZP;

/// <summary>
/// هویت دستگاه این مرورگر را با یک کوکی محافظت‌شده نگه می‌دارد و KioskId را از بانک دستگاه‌ها می‌خواند.
/// </summary>
public sealed class FaceKioskIdentity
{
    private const string CookieName = "Food.FaceKiosk";
    private const string DeviceRecordCookieName = "Food.FaceKiosk.DeviceRecord";
    private readonly IDataProtector _protector;

    public sealed record DeviceOption(long Id, string? Title, int KioskId, string? SerialNumber);

    public FaceKioskIdentity(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("Food.FaceKiosk.Identity.v1");
    }

    public async Task<int> GetOrCreateAsync(HttpContext context, ILogger logger,
        KioskRegistryOptions registryOptions)
    {
        var connectionString = registryOptions.ConnectionString;
        var deviceRecordId = GetPairedDeviceRecordId(context);
        if (string.IsNullOrWhiteSpace(connectionString) || !deviceRecordId.HasValue)
            throw new InvalidOperationException("این مرورگر هنوز به دستگاهی متصل نشده است.");

        int kioskId;
        await using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(context.RequestAborted);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT KioskId FROM dbo.Devices WHERE Id = @Id";
            command.Parameters.Add("@Id", System.Data.SqlDbType.BigInt).Value = deviceRecordId.Value;
            var result = await command.ExecuteScalarAsync(context.RequestAborted);
            if (result is null || result is DBNull ||
                !int.TryParse(Convert.ToString(result, CultureInfo.InvariantCulture), out kioskId) ||
                kioskId is < 100000 or > 999999)
                throw new InvalidOperationException($"رکورد دستگاه {deviceRecordId.Value} وجود ندارد یا KioskId شش‌رقمی معتبر ندارد.");
        }

        if (TryGet(context, out var current) && current == kioskId)
            return kioskId;

        logger.LogInformation(
            "هویت کیوسک مرورگر از بانک دستگاه‌ها خوانده شد. DeviceRecordId: {DeviceRecordId}, KioskId: {KioskId}",
            deviceRecordId.Value, kioskId);

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

    public async Task<IReadOnlyList<DeviceOption>> GetDevicesAsync(string connectionString,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("اتصال به بانک دستگاه‌ها در تنظیمات سامانه وارد نشده است.");

        var devices = new List<DeviceOption>();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, KioskId, SerialNumber FROM dbo.Devices ORDER BY Title, Id";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            devices.Add(new DeviceOption(reader.GetInt64(0),
                reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        }
        return devices;
    }

    public void PairBrowser(HttpContext context, long deviceRecordId)
    {
        if (deviceRecordId <= 0)
            throw new ArgumentOutOfRangeException(nameof(deviceRecordId));

        context.Response.Cookies.Append(DeviceRecordCookieName,
            _protector.Protect(deviceRecordId.ToString(CultureInfo.InvariantCulture)),
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
    }

    public long? GetPairedDeviceRecordId(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(DeviceRecordCookieName, out var value) ||
            string.IsNullOrWhiteSpace(value))
            return null;

        try
        {
            return long.TryParse(_protector.Unprotect(value), NumberStyles.None,
                CultureInfo.InvariantCulture, out var id) && id > 0 ? id : null;
        }
        catch (CryptographicException)
        {
            return null;
        }
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
