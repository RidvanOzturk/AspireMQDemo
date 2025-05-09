using AspireMQDemoWebApi.Services.Contracts;
using RabbitMQ.Client;
using Shared.Models;
using System.Text;
using System.Text.Json;

namespace AspireMQDemoWebApi.Services;

public class QueuePublisher : IQueuePublisher
{
    public async Task PublishAsync(QueueMessageModel message)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "product-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var payload = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "product-queue",
            body: body
        );
    }
}
