using Dapper;
using Npgsql;
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

var connectionString = "Host=localhost;Port=5432;Database=AspireDemoDb;Username=postgres;Password=fbr19072001";
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
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var queueMessage = JsonSerializer.Deserialize<QueueMessageModel>(message);
    await using var db = new NpgsqlConnection(connectionString);

    switch (queueMessage?.Operation)
    {
        case OperationType.Create:
            await db.ExecuteAsync(
              @"INSERT INTO ""Products"" (""Id"", ""Name"", ""Price"") VALUES (@Id, @Name, @Price)",
    new { queueMessage.Id, queueMessage.Name, queueMessage.Price });
            Console.WriteLine($"[Create] {queueMessage.Name} eklendi.");
            break;

        case OperationType.Update:
            await db.ExecuteAsync(
                 @"UPDATE ""Products"" SET ""Name"" = @Name, ""Price"" = @Price WHERE ""Id"" = @Id",
    new { queueMessage.Id, queueMessage.Name, queueMessage.Price });
            Console.WriteLine($"[Update] {queueMessage.Name} güncellendi.");
            break;

        case OperationType.Delete:
            await db.ExecuteAsync(
                @"DELETE FROM ""Products"" WHERE ""Id"" = @Id",
    new { queueMessage.Id });
            Console.WriteLine($"[Delete] ID: {queueMessage.Id} silindi.");
            break;

        default:
            Console.WriteLine("Not supported operation type!");
            break;
    }

};

await channel.BasicConsumeAsync("product-queue", autoAck: true, consumer: consumer);

Console.WriteLine("Press any button to end");
Console.ReadKey();

Console.WriteLine("Stopped reading queue!!!");