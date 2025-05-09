using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.AspireMQDemoWebApi>("AspireMQDemoWebApi");

builder.AddProject<Projects.AspireMQDemoWorker>("AspireMQDemoWorker");


builder.Build().Run();
