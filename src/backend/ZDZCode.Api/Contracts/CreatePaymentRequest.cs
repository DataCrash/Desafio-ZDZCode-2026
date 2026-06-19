using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CreatePaymentRequest
{
    public int OrderId { get; set; }

    [Required]
    [MaxLength(40)]
    public string Method { get; set; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Value { get; set; }

    [MaxLength(120)]
    public string? TransactionReference { get; set; }
}
