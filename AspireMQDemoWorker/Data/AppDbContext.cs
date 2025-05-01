using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using System.Collections.Generic;

namespace AspireMQDemoWorker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}