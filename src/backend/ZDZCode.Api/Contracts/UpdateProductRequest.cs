using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdateProductRequest
{
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
}
