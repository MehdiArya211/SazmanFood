using BLL.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.RedisService;

namespace BLL
{
    public class LogBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        //private readonly ILogger<BackgroundWorker> _logger;

        public LogBackgroundWorker(IServiceScopeFactory scopeFactory/*,ILogger<BackgroundWorker> logger*/)
        {
            _scopeFactory = scopeFactory;
            //_logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("{Type} is now running in the background.", nameof(BackgroundWorker));

            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogCritical(
            //    "The {Type} is stopping due to a host shutdown, queued items might not be processed anymore.",
            //    nameof(BackgroundWorker)
            //);

            return base.StopAsync(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(5000, stoppingToken);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var Redis = scope.ServiceProvider.GetRequiredService<IRedisManager>();
                        var logs = await Redis.db.PopLogs();
                        if (logs == null || !logs.Any()) continue;

                        var UserLogManager = scope.ServiceProvider.GetRequiredService<IUserLogManager>();
                        logs = logs.OrderBy(x => x.CreateDate).ToList();
                        var res = await UserLogManager.CreateRangeAsync(logs);
                        if (!res.Status)
                            await Redis.db.SetLogs(logs);
                    }
                }
                catch (Exception ex)
                {
                    //_logger.LogCritical("An error occurred when publishing a book. Exception: {@Exception}", ex);
                }
            }
        }
    }

}
