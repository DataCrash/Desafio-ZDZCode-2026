using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZDZCode.Api.Domain;

public sealed class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Required]
    [MaxLength(40)]
    public string Method { get; set; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    [MaxLength(120)]
    public string? TransactionReference { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Order? Order { get; set; }
}
