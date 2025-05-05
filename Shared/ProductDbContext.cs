using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using System.Collections.Generic;

namespace Shared;

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }
}
