using Shared.Entities;

namespace AspireMQDemoWebApi.Services.Contracts;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
}
