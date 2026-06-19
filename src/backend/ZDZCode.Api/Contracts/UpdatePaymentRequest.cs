using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdatePaymentRequest
{
    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? TransactionReference { get; set; }
}
