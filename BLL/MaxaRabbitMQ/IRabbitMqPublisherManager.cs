using DTO.Entities.MaxaRabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IRabbitMqPublisherManager
    {
        Task Publish<T>(T message);
        Task PublishToExchangeAsync<T>(string exchangeName, string routingKey, T message);
        Task PublishDeviceLockAsync(DeviceLockMessageDTO msg);

    }
}
