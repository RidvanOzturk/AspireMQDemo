using AspireMQDemoWebApi.Services;
using AspireMQDemoWebApi.Services.Contracts;
using AspireMQDemoWebApi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IQueuePublisher, QueuePublisher>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
