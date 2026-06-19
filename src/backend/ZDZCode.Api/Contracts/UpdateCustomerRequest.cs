using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdateCustomerRequest
{
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

    public CustomerAddressRequest? DeliveryAddress { get; set; }
}
