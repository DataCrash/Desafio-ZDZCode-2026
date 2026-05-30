using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Domain;

public sealed class Category
{
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
