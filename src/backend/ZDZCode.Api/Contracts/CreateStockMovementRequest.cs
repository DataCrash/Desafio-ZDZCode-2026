using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CreateStockMovementRequest
{
    public int ProductId { get; set; }

    [Required]
    [MaxLength(20)]
    public string MovementType { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [MaxLength(200)]
    public string? Reason { get; set; }
}
