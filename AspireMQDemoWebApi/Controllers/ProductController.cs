using AspireMQDemoWebApi.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using Shared.Enums;
using Shared.Models;

namespace AspireMQDemoWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IQueuePublisher queuePublisher, IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var id = Guid.NewGuid();

        var message = new QueueMessageModel
        {
            Id = id,
            Name = product.Name,
            Price = product.Price,
            Operation = OperationType.Create
        };

        await queuePublisher.PublishAsync(message);

        return Ok(new { id, message = "Product is queued for creation." });
    }

   [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }
}
