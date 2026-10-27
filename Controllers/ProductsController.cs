using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi_cs.Contracts;
using MyFirstApi_cs.Data;
using MyFirstApi_cs.Models;

namespace MyFirstApi_cs.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController(ProductDbContext database) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await database.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await database.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return product is null ? NotFound() : Ok(ToResponse(product));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
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

        var response = ToResponse(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await database.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        await database.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(product));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var product = await database.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        database.Products.Remove(product);
        await database.SaveChangesAsync(cancellationToken);
        return NoContent();
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