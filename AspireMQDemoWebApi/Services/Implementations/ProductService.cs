using AspireMQDemoWebApi.Services.Contracts;
using Dapper;
using Npgsql;
using Shared.Entities;

namespace AspireMQDemoWebApi.Services.Implementations;

public class ProductService(IConfiguration configuration) : IProductService
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection");

    public async Task<List<Product>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var sql = @"SELECT * FROM ""Products""";
        var result = await connection.QueryAsync<Product>(sql);
        return result.ToList();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var sql = @"SELECT * FROM ""Products"" WHERE ""Id"" = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var sql = @"UPDATE ""Products"" SET ""Name"" = @Name, ""Price"" = @Price WHERE ""Id"" = @Id";
        var result = await connection.ExecuteAsync(sql, product);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        var sql = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id";
        var result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }
}
