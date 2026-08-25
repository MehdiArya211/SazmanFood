namespace BLL
{
    public interface IRabbitMqConsumerManager
    {
        Task StartConsuming(CancellationToken cancellationToken);
    }
}
