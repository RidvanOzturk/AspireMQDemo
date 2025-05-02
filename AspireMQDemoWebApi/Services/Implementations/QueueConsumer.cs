using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using Shared.Enums;
using Shared.Entities;
using System;
using AspireMQDemoWorker.Data;

namespace AspireMQDemoWorker.Services.Implementations;
//ASK
public class QueueConsumer(IServiceProvider serviceProvider, IConfiguration configuration) : BackgroundService
{
    private IConnection? connection;
    private IModel? channel;

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost"
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "product-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeRabbitMQ();

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<QueueMessageModel<Product>>(body);

            if (message is not null && message.Operation == OperationType.Create)
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var product = new Product
                {
                    Id = message.Id,
                    Name = message.Data.Name,
                    Price = message.Data.Price
                };

                await dbContext.Products.AddAsync(product, stoppingToken);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        };

        channel.BasicConsume(queue: "product-queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        channel?.Close();
        connection?.Close();
        base.Dispose();
    }
}
