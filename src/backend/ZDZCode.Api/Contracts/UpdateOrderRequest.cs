using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdateOrderRequest
{
    [Required]
    [MaxLength(40)]
    public string Status { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal DiscountTotal { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}
