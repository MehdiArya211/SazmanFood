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

}

public class FaceAuthEventResult
{
    public bool IsSucces { get; set; }
    public string Message { get; set; }
    public long UserId { get; set; } // ✅ اضافه شد
}
