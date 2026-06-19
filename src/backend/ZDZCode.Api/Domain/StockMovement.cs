using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Domain;

public sealed class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Required]
    [MaxLength(20)]
    public string MovementType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}
