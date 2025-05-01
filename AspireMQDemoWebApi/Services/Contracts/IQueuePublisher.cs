using Shared.Models;

namespace AspireMQDemoWebApi.Services.Contracts;

public interface IQueuePublisher
{
    Task PublishAsync<T>(QueueMessageModel<T> message);
}
