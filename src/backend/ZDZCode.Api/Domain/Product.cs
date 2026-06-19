using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZDZCode.Api.Domain;

public sealed class Product
{
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(60)]
    public string? Sku { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(typeof(decimal), "0.01", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockCurrent { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
