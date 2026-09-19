using BLL;
using BLL.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Services.CookieServices;
using Services.RedisService;
using Services.SessionServices;

namespace Food.Hubs;

public class AuthHub : Hub
{
    private readonly IRedisManager _redis;
    private readonly IAuthManager _authManager;
    private readonly ILogger<AuthHub> _logger;

    public AuthHub(
        IRedisManager redis,
        IAuthManager authManager,
        ILogger<AuthHub> logger)
    {
        _redis = redis;
        _authManager = authManager;
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task NotifyAuthEvent(long deviceId)
    {
        var result = new FaceAuthEventResult();

        var userDevice = await _redis.db.GetRedisUserDeviceData(deviceId);
        if (userDevice == null)
        {
            result.IsSucces = false;
            result.Message = "در انتظار دستگاه تشخیص چهره ...";
        }
        else
        {
            result.IsSucces = true;
            result.Message = "ورود موفق! درحال انتقال به صفحه اصلی، لطفا شکیبا باشید";

            var user = _authManager.LoginWithFace(userDevice);
            if (user == null)
            {
                result.IsSucces = false;
                result.Message = "چهره شناسایی شد اما کاربر متناظر در سامانه یافت نشد.";

                _logger.LogWarning(
                    "رویداد تشخیص چهره دریافت شد اما کاربر پیدا نشد. DeviceId: {DeviceId}، PersonnelCode: {PersonnelCode}",
                    deviceId,
                    userDevice.UserId);
            }
            else
            {
                result.UserId = user.Id;

                var tk = await _redis.db.GetLoginToken(user.Id);
                if (tk == null)
                {
                    var token = await _redis.db.SetLoginToken(user.Id);
                    await _redis.db.RemoveLoginLog(user.Mobile);

                    var httpContext = Context.GetHttpContext();
                    httpContext.SetCookieUserToken(token);
                    httpContext.Session.SetUser(user);
                }

                _logger.LogInformation(
                    "ورود بیومتریک موفق شد. DeviceId: {DeviceId}، UserId: {UserId}",
                    deviceId,
                    user.Id);
            }
        }

        // پاسخ فقط برای همان مرورگری ارسال می‌شود که وضعیت دستگاه را درخواست کرده است.
        await Clients.Caller
            .SendAsync("NotifyAuthEvent", new
            {
                isSucces = result.IsSucces,
                message = result.Message,
                userId = result.UserId // 👈 در حالت camelCase برای جاوااسکریپت
            });
    }
}

public class FaceAuthEventResult
{
    public bool IsSucces { get; set; }
    public string Message { get; set; }
    public long UserId { get; set; } // ✅ اضافه شد
}
