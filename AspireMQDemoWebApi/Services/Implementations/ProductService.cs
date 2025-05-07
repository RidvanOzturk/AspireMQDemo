using AspireMQDemoWebApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Entities;

namespace AspireMQDemoWebApi.Services.Implementations;

public class ProductService(ProductDbContext productDbContext) : IProductService
{
    public Task<List<Product>> GetAllAsync()
    {
        return productDbContext.Products.ToListAsync();
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        return productDbContext.Products.FindAsync(id).AsTask();
    }
}
