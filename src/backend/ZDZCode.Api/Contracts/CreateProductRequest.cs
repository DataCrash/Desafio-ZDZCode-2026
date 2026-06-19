using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CreateProductRequest
{
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(60)]
    public string? Sku { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockCurrent { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
}
