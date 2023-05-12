using RabbitMQ.Client;
using System.Net;
using System.Text;

namespace TimeTeller.Services;

public class RabbitMQService
{
    private readonly string exchange;
    private readonly ConnectionFactory factory;

    public RabbitMQService(IConfiguration configuration)
    {
        exchange = configuration["RabbitMQ:Exchange"]!;
        factory = new ConnectionFactory { Uri = new Uri(configuration["RabbitMQ:ConnectionString"]!) };
    }

    public void PublishEndpointCalledMessage(string endpointName, IPAddress remoteAddress)
    {
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange, ExchangeType.Direct);
        var props = channel.CreateBasicProperties();
        props.UserId = factory.UserName;
        channel.BasicPublish(exchange, endpointName, body: Encoding.UTF8.GetBytes(remoteAddress.ToString()), basicProperties: props);
    }
}