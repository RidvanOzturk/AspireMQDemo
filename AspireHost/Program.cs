using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ container
var rabbitMq = builder.AddContainer("rabbitmq", "rabbitmq:3-management")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    .WithEndpoint(name: "amqp", port: 5672, targetPort: 5672)
    .WithEndpoint(name: "ui", port: 15672, targetPort: 15672);

// Web API
builder.AddProject<Projects.AspireMQDemoWebApi>("AspireMQDemoWebApi")
    .WithEnvironment("RabbitMQ__Host", "rabbitmq");

// Worker
builder.AddProject<Projects.AspireMQDemoWorker>("AspireMQDemoWorker")
    .WithEnvironment("RabbitMQ__Host", "rabbitmq");

builder.Build().Run();
