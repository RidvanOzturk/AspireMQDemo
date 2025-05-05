using AspireMQDemoWorker;

var consumer = new QueueConsumer();
await consumer.StartAsync();
