using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MyFirstApi_cs.Contracts;
using MyFirstApi_cs.Data;
using MyFirstApi_cs.Models;

namespace MyFirstApi_cs;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
    {
        var products = app.MapGroup("/api/products").WithTags("Products");

        products.MapGet("/", async Task<Ok<IReadOnlyList<ProductResponse>>> (
            ProductDbContext database,
            CancellationToken cancellationToken) =>
        {
            var items = await database.Products
                .AsNoTracking()
                .OrderBy(product => product.Id)
                .Select(product => ToResponse(product))
                .ToListAsync(cancellationToken);

            return TypedResults.Ok<IReadOnlyList<ProductResponse>>(items);
        })
        .WithName("GetProducts");

        products.MapGet("/{id:int}", async Task<Results<Ok<ProductResponse>, NotFound>> (
            int id,
            ProductDbContext database,
            CancellationToken cancellationToken) =>
        {
            var product = await database.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

            return product is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(ToResponse(product));
        })
        .WithName("GetProductById");

        products.MapPost("/", async Task<Created<ProductResponse>> (
            CreateProductRequest request,
            ProductDbContext database,
            CancellationToken cancellationToken) =>
        {
            var now = DateTimeOffset.UtcNow;
            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CreatedAt = now,
                UpdatedAt = now
            };

            database.Products.Add(product);
            await database.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/api/products/{product.Id}", ToResponse(product));
        })
        .WithName("CreateProduct");

        products.MapPut("/{id:int}", async Task<Results<Ok<ProductResponse>, NotFound>> (
            int id,
            UpdateProductRequest request,
            ProductDbContext database,
            CancellationToken cancellationToken) =>
        {
            var product = await database.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return TypedResults.NotFound();
            }

            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim();
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            await database.SaveChangesAsync(cancellationToken);
            return TypedResults.Ok(ToResponse(product));
        })
        .WithName("UpdateProduct");

        products.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (
            int id,
            ProductDbContext database,
            CancellationToken cancellationToken) =>
        {
            var product = await database.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return TypedResults.NotFound();
            }

            database.Products.Remove(product);
            await database.SaveChangesAsync(cancellationToken);
            return TypedResults.NoContent();
        })
        .WithName("DeleteProduct");

        return products;
    }

    private static ProductResponse ToResponse(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.StockQuantity,
        product.CreatedAt,
        product.UpdatedAt);
}