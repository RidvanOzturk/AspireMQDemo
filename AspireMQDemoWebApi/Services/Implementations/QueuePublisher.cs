using System.Text;
using System.Text.Json;
using AspireMQDemoWebApi.Services.Contracts;
using RabbitMQ.Client;
using Shared.Models;

namespace AspireMQDemoWebApi.Services.Implementations;
//ASK
public class QueuePublisher(IConfiguration configuration) : IQueuePublisher
{
    public Task PublishAsync<T>(QueueMessageModel<T> message)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost"
        };

        using var connection = factory.CreateConnection(); 
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "product-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        channel.BasicPublish(exchange: "", routingKey: "product-queue", basicProperties: null, body: body);

        return Task.CompletedTask;
    }
}
