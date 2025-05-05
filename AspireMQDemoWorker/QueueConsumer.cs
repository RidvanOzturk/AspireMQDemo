using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Entities;
using Shared.Models;
using System.Text.Json;

namespace AspireMQDemoWorker;

public class QueueConsumer
{
    public Task StartAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "product-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<QueueMessageModel<Product>>(body);

            if (message is not null)
            {
                Console.WriteLine($"[Worker] Received message: {message.Operation} - {message.Data.Name} - {message.Data.Price}₺");
            }
        };

        channel.BasicConsume(queue: "product-queue", autoAck: true, consumer: consumer);

        Console.WriteLine("[Worker] Listening to queue...");
        return Task.CompletedTask;
    }
}
