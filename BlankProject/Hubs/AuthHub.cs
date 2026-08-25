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

    public AuthHub(
        IRedisManager redis,
        IAuthManager authManager)
    {
        _redis = redis;
        _authManager = authManager;
    }

    public override async Task OnConnectedAsync()
    {
        var deviceId = 1; // TODO: change later
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId.ToString());
        await base.OnConnectedAsync();
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
            if (user != null)
            {
                result.UserId = user.Id; // ✅ اضافه شد

                var tk = await _redis.db.GetLoginToken(user.Id);
                if (tk == null)
                {
                    var token = await _redis.db.SetLoginToken(user.Id);
                    await _redis.db.RemoveLoginLog(user.Mobile);

                    var httpContext = Context.GetHttpContext();
                    httpContext.SetCookieUserToken(token);
                    httpContext.Session.SetUser(user);
                }
            }
        }

        // حالا به جای فقط IsSucces و Message، UserId هم می‌فرستیم
        await Clients.Group(deviceId.ToString())
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
