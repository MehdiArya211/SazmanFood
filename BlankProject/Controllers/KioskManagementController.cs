using System.ComponentModel.DataAnnotations;
using Domain.Constants;
using Food.Hubs.ZP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Services.SessionServices;

namespace Food.Controllers;

public sealed class KioskManagementController : Controller
{
    private readonly KioskRegistryOptions _options;
    private readonly FaceKioskIdentity _identity;

    public KioskManagementController(KioskRegistryOptions options, FaceKioskIdentity identity)
    {
        _options = options;
        _identity = identity;
    }

    public sealed class DeviceFormModel
    {
        public long Id { get; set; }
        [Required, Display(Name = "عنوان دستگاه")]
        public string? Title { get; set; }
        [Display(Name = "نوع اتصال")]
        public int ConnectType { get; set; }
        [Display(Name = "کلید اتصال")]
        public string? ConnectionKey { get; set; }
        [Display(Name = "برند دستگاه")]
        public int DeviceBrand { get; set; }
        [Display(Name = "نوع SDK")]
        public int SdkType { get; set; }
        [Display(Name = "پورت")]
        public int Port { get; set; }
        [Display(Name = "آدرس IP")]
        public string? Ip { get; set; }
        [Display(Name = "شناسه کیوسک")]
        public int KioskId { get; set; }
        [Display(Name = "شماره سریال")]
        public string? SerialNumber { get; set; }
        [Display(Name = "تأخیر")]
        public int? DelayTime { get; set; }
        [Display(Name = "شناسه فایل")]
        public long? FileForSaveId { get; set; }
        [Display(Name = "شناسه API")]
        public long? ApiId { get; set; }
        [Display(Name = "شناسه پایگاه داده")]
        public long? DBId { get; set; }
        [Display(Name = "حالت‌های شناسایی")]
        public string? EnIdentificationModes { get; set; }
        [Display(Name = "رنگ")]
        public int Color { get; set; }
        [Display(Name = "شناسه یکتا")]
        public Guid Guid { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "ایجادکننده")]
        public string? CreatedBy { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "آخرین ویرایشگر")]
        public string? LastModifyBy { get; set; }
    }

    public sealed class IndexModel
    {
        public IReadOnlyList<DeviceFormModel> Devices { get; init; } = [];
        public DeviceFormModel? Editing { get; init; }
        public long? PairedDeviceId { get; init; }
        public IReadOnlyList<FaceKioskIdentity.DeviceOption> PairOptions { get; init; } = [];
    }

    [HttpGet]
    public async Task<IActionResult> Index(long? editId, CancellationToken cancellationToken)
    {
        if (!HasKioskManagementPermission()) return Forbid();
        try
        {
            var devices = await LoadDevicesAsync(cancellationToken);
            var editing = editId.HasValue ? devices.FirstOrDefault(x => x.Id == editId.Value) : null;
            if (editId.HasValue && editing is null)
            {
                TempData["KioskError"] = "رکورد دستگاه پیدا نشد.";
            }
            return View(new IndexModel
            {
                Devices = devices,
                Editing = editing ?? new DeviceFormModel { Port = 5443, CreateDate = DateTime.Now, LastModifyDate = DateTime.Now, Guid = Guid.NewGuid() },
                PairedDeviceId = _identity.GetPairedDeviceRecordId(HttpContext),
                PairOptions = await _identity.GetDevicesAsync(_options.ConnectionString!, cancellationToken)
            });
        }
        catch (Exception ex)
        {
            TempData["KioskError"] = "خواندن اطلاعات بانک دستگاه‌ها انجام نشد: " + ex.Message;
            return View(new IndexModel
            {
                Editing = new DeviceFormModel { Port = 5443, CreateDate = DateTime.Now, LastModifyDate = DateTime.Now, Guid = Guid.NewGuid() },
                PairedDeviceId = _identity.GetPairedDeviceRecordId(HttpContext)
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PairBrowser(long deviceRecordId, CancellationToken cancellationToken)
    {
        if (!HasKioskManagementPermission()) return Forbid();
        try
        {
            var device = (await _identity.GetDevicesAsync(_options.ConnectionString!, cancellationToken))
                .FirstOrDefault(x => x.Id == deviceRecordId);
            if (device is null || device.KioskId <= 0)
            {
                TempData["KioskError"] = "دستگاه انتخاب‌شده پیدا نشد یا شناسه کیوسک آن معتبر نیست.";
            }
            else
            {
                _identity.PairBrowser(HttpContext, deviceRecordId);
                TempData["KioskMessage"] = $"این مرورگر به «{device.Title}» متصل شد. شناسه کیوسک: {device.KioskId}";
            }
        }
        catch (Exception ex)
        {
            TempData["KioskError"] = "اتصال مرورگر به دستگاه انجام نشد: " + ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(DeviceFormModel model, CancellationToken cancellationToken)
    {
        if (!HasKioskManagementPermission()) return Forbid();
        if (!ModelState.IsValid || model.KioskId <= 0)
        {
            TempData["KioskError"] = "عنوان دستگاه و شناسه کیوسک معتبر را وارد کنید.";
            return model.Id > 0
                ? RedirectToAction(nameof(Index), new { editId = model.Id })
                : RedirectToAction(nameof(Index));
        }

        try
        {
            await SaveDeviceAsync(model, cancellationToken);
            TempData["KioskMessage"] = model.Id > 0 ? "اطلاعات دستگاه ذخیره شد." : "دستگاه جدید ثبت شد.";
        }
        catch (Exception ex)
        {
            TempData["KioskError"] = "ذخیره اطلاعات دستگاه انجام نشد: " + ex.Message;
            return model.Id > 0
                ? RedirectToAction(nameof(Index), new { editId = model.Id })
                : RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }

    private bool HasKioskManagementPermission()
    {
        var user = HttpContext.Session.GetUser();
        return user?.IsEnabled == true && user.RoleId == RoleConstant.Admin;
    }

    private async Task<List<DeviceFormModel>> LoadDevicesAsync(CancellationToken cancellationToken)
    {
        var rows = new List<DeviceFormModel>();
        await using var connection = new SqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, ConnectType, ConnectionKey, DeviceBrand, SdkType, Port, Ip, KioskId, SerialNumber, DelayTime, FileForSaveId, ApiId, DBId, EnIdentificationModes, Color, Guid, CreateDate, CreatedBy, LastModifyDate, LastModifyBy FROM dbo.Devices ORDER BY Title, Id";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            rows.Add(new DeviceFormModel
            {
                Id = reader.GetInt64(0), Title = Text(reader, 1), ConnectType = reader.GetInt32(2), ConnectionKey = Text(reader, 3),
                DeviceBrand = reader.GetInt32(4), SdkType = reader.GetInt32(5), Port = reader.GetInt32(6), Ip = Text(reader, 7),
                KioskId = reader.GetInt32(8), SerialNumber = Text(reader, 9), DelayTime = NullableInt(reader, 10),
                FileForSaveId = NullableLong(reader, 11), ApiId = NullableLong(reader, 12), DBId = NullableLong(reader, 13),
                EnIdentificationModes = Text(reader, 14), Color = reader.GetInt32(15), Guid = reader.GetGuid(16),
                CreateDate = reader.GetDateTime(17), CreatedBy = Text(reader, 18), LastModifyDate = reader.GetDateTime(19),
                LastModifyBy = Text(reader, 20)
            });
        }
        return rows;
    }

    private async Task SaveDeviceAsync(DeviceFormModel model, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        if (model.Id > 0)
        {
            command.CommandText = "UPDATE dbo.Devices SET Title=@Title, ConnectType=@ConnectType, ConnectionKey=@ConnectionKey, DeviceBrand=@DeviceBrand, SdkType=@SdkType, Port=@Port, Ip=@Ip, KioskId=@KioskId, SerialNumber=@SerialNumber, DelayTime=@DelayTime, FileForSaveId=@FileForSaveId, ApiId=@ApiId, DBId=@DBId, EnIdentificationModes=@EnIdentificationModes, Color=@Color, LastModifyDate=SYSDATETIME(), LastModifyBy=@LastModifyBy WHERE Id=@Id";
            command.Parameters.AddWithValue("@Id", model.Id);
        }
        else
        {
            command.CommandText = "INSERT INTO dbo.Devices (Title, ConnectType, ConnectionKey, DeviceBrand, SdkType, Port, Ip, KioskId, SerialNumber, DelayTime, FileForSaveId, ApiId, DBId, EnIdentificationModes, Color, Guid, CreateDate, CreatedBy, LastModifyDate, LastModifyBy) VALUES (@Title, @ConnectType, @ConnectionKey, @DeviceBrand, @SdkType, @Port, @Ip, @KioskId, @SerialNumber, @DelayTime, @FileForSaveId, @ApiId, @DBId, @EnIdentificationModes, @Color, @Guid, SYSDATETIME(), @CreatedBy, SYSDATETIME(), @LastModifyBy)";
            command.Parameters.AddWithValue("@Guid", model.Guid == Guid.Empty ? Guid.NewGuid() : model.Guid);
            AddNullable(command, "@CreatedBy", model.CreatedBy);
        }
        command.Parameters.AddWithValue("@Title", model.Title!.Trim());
        command.Parameters.AddWithValue("@ConnectType", model.ConnectType);
        AddNullable(command, "@ConnectionKey", model.ConnectionKey);
        command.Parameters.AddWithValue("@DeviceBrand", model.DeviceBrand);
        command.Parameters.AddWithValue("@SdkType", model.SdkType);
        command.Parameters.AddWithValue("@Port", model.Port);
        AddNullable(command, "@Ip", model.Ip);
        command.Parameters.AddWithValue("@KioskId", model.KioskId);
        AddNullable(command, "@SerialNumber", model.SerialNumber);
        AddNullable(command, "@DelayTime", model.DelayTime);
        AddNullable(command, "@FileForSaveId", model.FileForSaveId);
        AddNullable(command, "@ApiId", model.ApiId);
        AddNullable(command, "@DBId", model.DBId);
        AddNullable(command, "@EnIdentificationModes", model.EnIdentificationModes);
        command.Parameters.AddWithValue("@Color", model.Color);
        AddNullable(command, "@LastModifyBy", model.LastModifyBy);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddNullable(SqlCommand command, string name, object? value) =>
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    private static string? Text(SqlDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    private static int? NullableInt(SqlDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    private static long? NullableLong(SqlDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetInt64(ordinal);
}
