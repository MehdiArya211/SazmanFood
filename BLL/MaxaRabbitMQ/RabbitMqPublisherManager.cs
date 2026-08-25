// BLL/RabbitMqPublisherManager.cs
using DTO.Entities;
using DTO.Entities.MaxaRabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BLL
{
    public class RabbitMqPublisherManager : IRabbitMqPublisherManager
    {
        private readonly string _exchangeName;
        private readonly BusConfigDTO _busConfig;

        public RabbitMqPublisherManager(BusConfigDTO busConfig)
        {
            _exchangeName = busConfig.ExchangeName;
            _busConfig = busConfig;
        }

        // متد عمومی برای هر اکسچنج
        public async Task PublishToExchangeAsync<T>(string exchangeName, string routingKey, T message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _busConfig.Host,
                    Port = _busConfig.Port,
                    UserName = _busConfig.Username,
                    Password = _busConfig.Password
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // معمولاً Fanout برای برادکست – اگر Direct لازم داری، تغییر بده
                await channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: ExchangeType.Fanout,
                    durable: true
                );

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKey ?? string.Empty,
                    body: body
                );

                Console.WriteLine($"✅ Sent to {exchangeName}: {JsonConvert.SerializeObject(message)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در ارسال پیام به RabbitMQ({exchangeName}): {ex.Message}");
            }
        }

        // متد فعلی چاپگر برای Backward-compat
        public async Task Publish<T>(T message)
            => await PublishToExchangeAsync("Dima_Printer", "Dima_Printer", message);

        // شورت‌کات مخصوص قفل دستگاه
        public Task PublishDeviceLockAsync(DeviceLockMessageDTO msg)
            => PublishToExchangeAsync("Dima_Lock", "Dima_Lock", msg);
    }
}
