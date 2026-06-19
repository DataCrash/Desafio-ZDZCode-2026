using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZDZCode.Api.Domain;

public sealed class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = "draft";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
