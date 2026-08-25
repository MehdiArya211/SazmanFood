using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Food.Hubs
{
    public class KioskHub : Hub
    {
        // kioskId => (connectionId, lastSeenUtc)
        private static readonly ConcurrentDictionary<int, KioskConnectionState> _kiosks = new();

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"[KioskHub] Connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        // کیوسک خودش را معرفی می‌کند
        public Task Register(int kioskId)
        {
            _kiosks[kioskId] = new KioskConnectionState
            {
                ConnectionId = Context.ConnectionId,
                LastSeenUtc = DateTime.UtcNow,
                IsConnected = true
            };

            Console.WriteLine($"[KioskHub] Kiosk [{kioskId}] registered with ConnectionId [{Context.ConnectionId}]");
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var item = _kiosks.FirstOrDefault(x => x.Value.ConnectionId == Context.ConnectionId);

            if (item.Key != 0)
            {
                // حذف نمی‌کنیم، فقط وضعیت را disconnected می‌کنیم
                _kiosks[item.Key] = item.Value with
                {
                    LastSeenUtc = DateTime.UtcNow,
                    IsConnected = false
                };

                Console.WriteLine($"[KioskHub] Kiosk [{item.Key}] disconnected (temporary).");
            }

            return base.OnDisconnectedAsync(exception);
        }

        // API از این متد برای پیدا کردن ConnectionId استفاده می‌کند
        public static bool TryGetConnection(int kioskId, out string connectionId)
        {
            connectionId = string.Empty;

            if (!_kiosks.TryGetValue(kioskId, out var state))
                return false;

            // اگر خیلی قدیمی است می‌توانی false برگردانی یا پاک کنی (اختیاری)
            // مثلا اگر 2 دقیقه از آخرین اتصال گذشته:
            // if (DateTime.UtcNow - state.LastSeenUtc > TimeSpan.FromMinutes(2)) return false;

            connectionId = state.ConnectionId;
            return !string.IsNullOrWhiteSpace(connectionId);
        }

        private record KioskConnectionState
        {
            public string ConnectionId { get; init; } = "";
            public DateTime LastSeenUtc { get; init; }
            public bool IsConnected { get; init; }
        }
    }
}
