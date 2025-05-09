using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Enums;
using Shared.Models;
using System.Text;
using System.Text.Json;


Console.WriteLine("Started reading queue!!!");

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

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var queueMessage = JsonSerializer.Deserialize<QueueMessageModel>(message);

    switch (queueMessage?.Operation)
    {   
        case OperationType.Create:
            break;
        case OperationType.Update:
            break;
        case OperationType.Delete:
            break;
        default:
            Console.WriteLine("Not supported operation type!");
            break;
    }

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("product-queue", autoAck: true, consumer: consumer);

Console.WriteLine("Press any button to end");
Console.ReadKey();

Console.WriteLine("Stopped reading queue!!!");