using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Entities;
using Shared.Models;
using Shared.Enums;
using System.Text.Json;
using System.Text;

namespace AspireMQDemoWorker;

public class QueueConsumer
{
    public Task StartAsync()
    {
        var configuration = new ConfigurationManager()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

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

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<QueueMessageModel<Product>>(body);

            if (message is not null && message.Operation == OperationType.Create)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
                optionsBuilder.UseNpgsql(connectionString);

                using var dbContext = new ProductDbContext(optionsBuilder.Options);

                var product = new Product
                {
                    Id = message.Id,
                    Name = message.Data.Name,
                    Price = message.Data.Price
                };

                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"[Worker] ✅ Saved to DB: {product.Name} - {product.Price}₺");
            }
        };

        channel.BasicConsume(queue: "product-queue", autoAck: true, consumer: consumer);

        Console.WriteLine("[Worker] Listening to queue...");
        return Task.CompletedTask;
    }
}
