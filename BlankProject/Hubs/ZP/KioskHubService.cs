using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;

namespace Food.Hubs.ZP;
public class KioskHubService
{
    private readonly HubConnection _hubConnection;

    public string? ConnectionId => _hubConnection?.ConnectionId;

    //public event Func<string, Task>? OnReceiveAutoLogin;

    public KioskHubService(NavigationManager navigationManager, IConfiguration configuration)
    {
        // ساخت Connection به Hub
        var url = configuration.GetValue<string>("ApiAddress:Refit");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri($"{url}/kioskHub"), options =>
            {
                // فعال کردن WebSockets و LongPolling
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();


        // Event داخلی Hub
        //_hubConnection.On<string>("ReceiveAutoLogin", async (token) =>
        //{
        //    if (OnReceiveAutoLogin != null)
        //        await OnReceiveAutoLogin.Invoke(token);
        //});
    }


    //public async Task OnReceiveAutoLogin(Action<string> payload)
    //{
    //    try
    //    {
    //        _hubConnection.On<string>("ReceiveAutoLogin", payload);

    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}
    // اتصال و رجیستر کیوسک
    public async Task Connect(int kioskId)
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
            Console.WriteLine($"KioskHub Connected: {_hubConnection.ConnectionId}");

            // فراخوانی Register در سرور
            await _hubConnection.InvokeAsync("Register", kioskId);
            Console.WriteLine($"Kiosk [{kioskId}] registered on KioskHub");
        }
    }

    public async Task Disconnect()
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }
    //public void RegisterAutoLoginHandler(Func<string, Task> handler)
    //{
    //    _hubConnection.On<string>("ReceiveAutoLogin", async enrollId =>
    //    {
    //        await handler(enrollId);
    //    });
    //}



    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.DisposeAsync();
    }
    public void RegisterAutoLoginHandler(Action<string> action)
    {
        try
        {
            _hubConnection.On<string>("ReceiveAutoLogin", action);

        }
        catch (Exception ex)
        {

        }
    }
}
