using System.ComponentModel.DataAnnotations;

namespace MyFirstApi_cs.Contracts;

/// <summary>Payload for creating a product.</summary>
public sealed record CreateProductRequest
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
}

/// <summary>Payload for replacing a product's editable fields.</summary>
public sealed record UpdateProductRequest
{
    [Required, MaxLength(200)]
    public required string Name { get; init; }

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
}

/// <summary>A product returned by the API.</summary>
public sealed record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);