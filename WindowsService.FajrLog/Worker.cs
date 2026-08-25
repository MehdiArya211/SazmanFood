using BLL.FajrLog;
using Services.RedisService;

namespace WindowsService.FajrLog
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var Redis = scope.ServiceProvider.GetRequiredService<IRedisManager>();
                    var logs = await Redis.db.PopFajrLogs();
                    if (logs == null || !logs.Any()) continue;

                    var fajrLogManager = scope.ServiceProvider.GetRequiredService<IFajrLogManager>();
                    logs = logs.OrderBy(x => x.logNum).ToList();
                    var isSuccess = fajrLogManager.CreateRange(logs);
                    if (!isSuccess)
                        await Redis.db.SetFajrLogs(logs);

                    _logger.LogInformation("*********** Fajr Log Service ==> log count is {logCount} ==> running at: {time}", logs.Count(), DateTimeOffset.Now);
                    
                }
                await Task.Delay(120000, stoppingToken);
            }
        }
    }
}