using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Enums;
using AspireMQDemoWebApi.Models;
using AspireMQDemoWebApi.Services.Contracts;
using AspireMQDemoWebApi.Services.Implementations;
using Shared.Entities;

namespace AspireMQDemoWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IQueuePublisher queuePublisher, IProductService productService) : ControllerBase
{
    // POST /api/products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var id = Guid.NewGuid();

        var message = new QueueMessageModel<Product>
        {
            Id = id,
            Operation = OperationType.Create,
            Data = product
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
