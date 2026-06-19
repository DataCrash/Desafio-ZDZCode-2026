namespace ZDZCode.Api.Contracts;

public sealed class OrderResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Note { get; set; }
    public IReadOnlyList<OrderItemResponse> Items { get; set; } = [];
}
