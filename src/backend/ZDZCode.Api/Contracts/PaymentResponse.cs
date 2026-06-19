namespace ZDZCode.Api.Contracts;

public sealed class PaymentResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime CreatedAt { get; set; }
}
