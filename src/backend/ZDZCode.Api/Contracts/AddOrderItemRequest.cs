using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class AddOrderItemRequest
{
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
