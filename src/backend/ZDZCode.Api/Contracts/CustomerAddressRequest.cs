using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CustomerAddressRequest
{
    [Required]
    [MaxLength(180)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? District { get; set; }

    [Required]
    [MaxLength(120)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(12)]
    public string ZipCode { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Complement { get; set; }
}
