using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Food.Hubs.ZP;

/// <summary>
/// Bridges the server's connection to the internal ZP hub to the browser on the site's own origin.
/// The old ZP hub accepts Register(kioskId) and emits ReceiveAutoLogin(string).
/// </summary>
public sealed class FaceZpRelayHub : Hub
{
    private static readonly ConcurrentDictionary<string, HubConnection> ZpConnections = new();

    private readonly IConfiguration _configuration;
    private readonly IHubContext<FaceZpRelayHub> _browserHub;
    private readonly ILogger<FaceZpRelayHub> _logger;

    public FaceZpRelayHub(
        IConfiguration configuration,
        IHubContext<FaceZpRelayHub> browserHub,
        ILogger<FaceZpRelayHub> logger)
    {
        _configuration = configuration;
        _browserHub = browserHub;
        _logger = logger;
    }

    public async Task Register(int kioskId)
    {
        var configuredKioskId = _configuration.GetValue<int?>("ApiAddress:KioskId");
        if (configuredKioskId.GetValueOrDefault() <= 0 ||
            kioskId != configuredKioskId.Value)
            throw new HubException("شناسه دستگاه ZP معتبر نیست.");

        var baseUrl = _configuration["ApiAddress:Refit"]?.TrimEnd('/');
        if (!Uri.TryCreate($"{baseUrl}/kioskHub", UriKind.Absolute, out var hubUri) ||
            (hubUri.Scheme != Uri.UriSchemeHttp && hubUri.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(baseUrl))
            throw new HubException("آدرس سرویس ZP در تنظیمات سامانه معتبر نیست.");

        var browserConnectionId = Context.ConnectionId;
        if (ZpConnections.TryRemove(browserConnectionId, out var previous))
            await previous.DisposeAsync();

        var zpConnection = new HubConnectionBuilder()
            .WithUrl(hubUri, options =>
            {
                options.Transports =
                    HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();

        zpConnection.On<string>("ReceiveAutoLogin", enrollId =>
            _browserHub.Clients.Client(browserConnectionId)
                .SendAsync("ReceiveAutoLogin", enrollId));

        zpConnection.On<object>("KioskDiagnostic", diagnostic =>
            _browserHub.Clients.Client(browserConnectionId)
                .SendAsync("KioskDiagnostic", diagnostic));

        zpConnection.Reconnecting += async error =>
        {
            _logger.LogWarning(error, "اتصال ZP برای کیوسک {KioskId} قطع شد.", kioskId);
            await _browserHub.Clients.Client(browserConnectionId)
                .SendAsync("ZpConnectionStatus", false);
        };

        zpConnection.Reconnected += async _ =>
        {
            try
            {
                await zpConnection.InvokeAsync("Register", kioskId);
                await _browserHub.Clients.Client(browserConnectionId)
                    .SendAsync("ZpConnectionStatus", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ثبت مجدد کیوسک {KioskId} در ZP ناموفق بود.", kioskId);
                await _browserHub.Clients.Client(browserConnectionId)
                    .SendAsync("ZpConnectionStatus", false);
            }
        };

        if (!ZpConnections.TryAdd(browserConnectionId, zpConnection))
        {
            await zpConnection.DisposeAsync();
            throw new HubException("ثبت اتصال دستگاه ZP ناموفق بود.");
        }

        try
        {
            await zpConnection.StartAsync(Context.ConnectionAborted);
            await zpConnection.InvokeAsync("Register", kioskId);
            _logger.LogInformation(
                "کیوسک {KioskId} از سرور به ZP متصل شد. BrowserConnectionId: {ConnectionId}",
                kioskId, browserConnectionId);
        }
        catch (Exception ex)
        {
            ZpConnections.TryRemove(browserConnectionId, out _);
            await zpConnection.DisposeAsync();
            _logger.LogError(ex, "اتصال یا ثبت کیوسک {KioskId} در ZP ناموفق بود.", kioskId);
            throw new HubException("سرور برنامه نتوانست به سرویس ZP متصل شود یا دستگاه را ثبت کند.");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ZpConnections.TryRemove(Context.ConnectionId, out var zpConnection))
            await zpConnection.DisposeAsync();

        await base.OnDisconnectedAsync(exception);
    }
}
