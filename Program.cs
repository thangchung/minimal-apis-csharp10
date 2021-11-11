using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SampleMinimalWebApi;

// after enabled global usings, then we can remove above line
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/globalusingdirective

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration; // this is ConfigurationManager, we don't care too much what is for WebHost, what is for HostBuilder
// https://andrewlock.net/exploring-dotnet-6-part-1-looking-inside-configurationmanager-in-dotnet-6/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // DI for Minimal APIs

var app = builder.Build();

// we don't have to handle what is development so that we will handle exception, Minimal APIs will do it in core
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Look at the Lambda Expression below, in .NET 6 with C# 10, it's improved a lot for method group and delegation
// now it will auto infer type when returns on the body
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/lambda-improvements
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Allow attach attributes to Lambda Expression like [FromBody], [FromQuery], [FromHeader]...
// and with Minimal APIs, we don't need to have [FromServices] for DI as well
app.MapGet("/products/{id:int}",
    async (int id, [FromServices] IProductRepository productRepository) =>
    {
        return await productRepository.GetProduct(id) switch // improve patterns matching in C# 10
        {
            { } product => Results.Ok(product), // { } is null check in C# 10
            null => Results.NotFound()
        };
    });

app.MapPost("/products",
    (AddProductModel product) =>
    {
        // if you using Global Usings Static then you can call RandomName() without RandomHelper
        var createdProduct = product with {Id = RandomHelper.RandomNumber(), Name = RandomHelper.RandomName()};
        Console.WriteLine(JsonSerializer.Serialize(createdProduct));
        return Results.Ok(createdProduct);
    });

// with Record Struct in C# 10 we can using With for minimize the assignment needs to assign id into the model neatly
app.MapPut("/products/{id:int}",
    (int id, [FromBody] UpdateProductModel model) => Results.Ok(model with {Id = id}));

// we can map fallback to swagger like this
app.MapFallback(() => Results.Redirect("/swagger"));

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
}

internal class ProductRepository : IProductRepository
{
    public Task<Product?> GetProduct(int id)
    {
        Product? product = null;

        if(id == 1 && product is not { }) // Null check in C# 10
        {
            product = new Product(1, "Sample 01");
        }

        return Task.FromResult(product);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        var products = new List<Product> {new Product(1, "Sample 01") };
        return await Task.FromResult(products);
    }
}

record struct Product(int Id, string Name, Category? Category = null);

record Category(int Id, string Name);

record struct AddProductModel(int Id, string Name, Category? Category = null);
record struct UpdateProductModel(int Id, string Name, Category? Category = null);
