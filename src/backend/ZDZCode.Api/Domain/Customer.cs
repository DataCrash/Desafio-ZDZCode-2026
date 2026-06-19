using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Domain;

public sealed class Customer
{
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DeliveryAddress? DeliveryAddress { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
