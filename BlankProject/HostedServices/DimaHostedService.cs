using BLL;

namespace Food.HostedServices
{
    public class DimaHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DimaHostedService> _logger;

        public DimaHostedService(IServiceProvider serviceProvider, ILogger<DimaHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumerManager>();

                consumer.StartConsuming(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطا در RabbitMqBackgroundConsumer");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer stopped.");
            return Task.CompletedTask;
        }

    }
}
