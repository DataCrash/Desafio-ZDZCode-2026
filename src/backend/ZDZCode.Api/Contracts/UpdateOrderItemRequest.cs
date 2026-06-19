using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdateOrderItemRequest
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
