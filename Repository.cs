// Refactor and move Repository our of Program.cs to demo the file-scoped namespace

namespace SampleMinimalWebApi;
// Above is file scoped namespace
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/file-scoped-namespaces

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
}

internal class ProductRepository : IProductRepository
{
    private const string Prefix = ".NET Conf 2021";
    private const string ProductPrefix = $"{Prefix} - Product"; // C# 10: Constant Interpolated Strings

    public Task<Product?> GetProduct(int id)
    {
        Product? product = null;
        if (id == 1 && product is not { }) // Null check in C# 10
        {
            product = new Product(1, $"{ProductPrefix}{id}");
        }

        return Task.FromResult(product);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        var products = new List<Product> {new Product(1, "Sample 01") };
        return await Task.FromResult(products);
    }
}

// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/record-structs
public record struct Product(int Id, string Name, Category? Category = null);

public record Category(int Id, string Name);
