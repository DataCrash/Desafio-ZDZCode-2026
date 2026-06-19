namespace ZDZCode.Api.Contracts;

public sealed class StockMovementResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
