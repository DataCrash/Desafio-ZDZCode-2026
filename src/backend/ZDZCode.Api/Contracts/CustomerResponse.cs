namespace ZDZCode.Api.Contracts;

public sealed class CustomerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DeliveryAddressResponse? DeliveryAddress { get; set; }
}
