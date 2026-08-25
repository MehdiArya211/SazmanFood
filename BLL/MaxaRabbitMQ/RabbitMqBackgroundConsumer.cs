using BLL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RabbitMqBackgroundConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqBackgroundConsumer> _logger;

    public RabbitMqBackgroundConsumer(IServiceProvider serviceProvider, ILogger<RabbitMqBackgroundConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumerManager>();

            //await consumer.StartConsuming(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ خطا در RabbitMqBackgroundConsumer");
        }
    }
}
