using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Enums;
using AspireMQDemoWebApi.Models;
using AspireMQDemoWebApi.Services.Contracts;

namespace AspireMQDemoWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IQueuePublisher queuePublisher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductModel model)
    {
        var message = new QueueMessageModel<ProductModel>
        {
            Id = Guid.NewGuid(),
            Operation = OperationType.Create,
            Data = model
        };

        await queuePublisher.PublishAsync(message);

        return Accepted(new { message.Id });
    }
}
