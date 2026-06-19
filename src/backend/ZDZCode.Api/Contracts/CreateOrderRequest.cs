using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CreateOrderRequest
{
    public int CustomerId { get; set; }

    [MaxLength(40)]
    public string Status { get; set; } = "draft";

    [Range(typeof(decimal), "0", "9999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal DiscountTotal { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}
