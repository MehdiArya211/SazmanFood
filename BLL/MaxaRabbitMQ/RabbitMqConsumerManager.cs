using DTO.Entities;
using DTO.Entities.MaxaRabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.RedisService;
using System.Text;
using System.Text.Json;

namespace BLL
{
    public class RabbitMqConsumerManager : IRabbitMqConsumerManager
    {
        private readonly string _exchangeName;
        private readonly BusConfigDTO _busConfig;
        private readonly IRabbitMqPublisherManager _publisher;
        private readonly IRedisManager _redis;

        public RabbitMqConsumerManager(
            BusConfigDTO busConfig,
            IRabbitMqPublisherManager publisher,
            IRedisManager redis)
        {
            _exchangeName = busConfig.ExchangeName;
            _busConfig = busConfig;
            _publisher = publisher;
            _redis = redis;
        }

        private static readonly HttpClient httpClient = new HttpClient();

        public async Task StartConsuming(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚀 StartConsuming شروع شد...");
            var factory = new ConnectionFactory()
            {
                HostName = _busConfig.Host,
                Port = _busConfig.Port,
                UserName = _busConfig.Username,
                Password = _busConfig.Password
            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(queue: "AccessLogMonitoringEvent", durable: true, exclusive: false, autoDelete: false);
            Console.WriteLine($"📦 صف AccessLogMonitoringEvent موجود است، تعداد پیام‌ها: {queueDeclareResult.MessageCount}");
            string queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queue: queueName, exchange: "AccessLogMonitoringEvent", routingKey: "");
            var consumer = new AsyncEventingBasicConsumer(channel); consumer.ReceivedAsync += async (model, ea) => { Console.WriteLine("📩 پیام جدید دریافت شد → ورود به OnMessageReceived..."); await OnMessageReceived(model, ea, channel); };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine("👂 در حال گوش دادن به پیام‌های RabbitMQ...");
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea, IChannel channel)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var data = JsonSerializer.Deserialize<AccessLogMonitoringEvent>(message);

            Console.WriteLine("📩 Received message:");
            Console.WriteLine($" Employee ID: {data.UserId}");
            Console.WriteLine($" Timestamp: {data.EventDateTime}");
            Console.WriteLine($" Device ID: {data.DeviceId}");
            Console.WriteLine($" Template Type: {data.AuthMethod}");

            if (data.UserId == 0)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                return;
            }

            try
            {
                if (await _redis.db.GetRedisUserDeviceData(data.DeviceId) == null)
                {
                    await _redis.db.SetRedisUserDeviceData(data);
                }
                
                Console.WriteLine($"➡️ FaceAuth request sent for UserId {data.UserId}");

                // پیام RabbitMQ تأیید میشه
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling FaceAuthController: {ex.Message}");
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
        }
    }
}
