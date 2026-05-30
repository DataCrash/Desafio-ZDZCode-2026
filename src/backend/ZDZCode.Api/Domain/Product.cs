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

    [Column(TypeName = "decimal(18,2)")]
    [Range(typeof(decimal), "0.01", "9999999999")]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}
