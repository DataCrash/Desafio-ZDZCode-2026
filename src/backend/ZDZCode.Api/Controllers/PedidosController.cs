using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public sealed class PedidosController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(CancellationToken ct)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

        return Ok(orders.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var customerExists = await dbContext.Customers.AnyAsync(x => x.Id == request.CustomerId, ct);
        if (!customerExists)
        {
            return BadRequest(new { message = "Cliente invalido para o pedido." });
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "draft" : request.Status.Trim(),
            DiscountTotal = request.DiscountTotal,
            Note = request.Note?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        RecalculateTotals(order);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(ct);

        var created = await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstAsync(x => x.Id == order.Id, ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderResponse>> Update(int id, UpdateOrderRequest request, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order is null)
        {
            return NotFound();
        }

        order.Status = request.Status.Trim();
        order.DiscountTotal = request.DiscountTotal;
        order.Note = request.Note?.Trim();
        order.UpdatedAt = DateTime.UtcNow;

        RecalculateTotals(order);
        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(order));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null)
        {
            return NotFound();
        }

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:int}/itens")]
    public async Task<ActionResult<OrderResponse>> AddItem(int id, AddOrderItemRequest request, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order is null)
        {
            return NotFound(new { message = "Pedido nao encontrado." });
        }

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (product is null)
        {
            return BadRequest(new { message = "Produto invalido para o item." });
        }

        if (product.StockCurrent < request.Quantity)
        {
            return Conflict(new { message = "Estoque insuficiente para incluir item no pedido." });
        }

        var alreadyExists = order.Items.Any(x => x.ProductId == request.ProductId);
        if (alreadyExists)
        {
            return Conflict(new { message = "Produto ja adicionado ao pedido." });
        }

        var item = new OrderItem
        {
            OrderId = order.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            LineTotal = product.Price * request.Quantity
        };

        order.Items.Add(item);
        product.StockCurrent -= request.Quantity;
        dbContext.StockMovements.Add(new StockMovement
        {
            ProductId = product.Id,
            MovementType = "saida",
            Quantity = request.Quantity,
            Reason = $"Saida automatica do pedido {order.Id}",
            CreatedAt = DateTime.UtcNow
        });

        order.UpdatedAt = DateTime.UtcNow;
        RecalculateTotals(order);

        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(order));
    }

    [HttpPut("{id:int}/itens/{itemId:int}")]
    public async Task<ActionResult<OrderResponse>> UpdateItem(int id, int itemId, UpdateOrderItemRequest request, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order is null)
        {
            return NotFound(new { message = "Pedido nao encontrado." });
        }

        var item = order.Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return NotFound(new { message = "Item do pedido nao encontrado." });
        }

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId, ct);
        if (product is null)
        {
            return BadRequest(new { message = "Produto do item nao encontrado." });
        }

        var quantityDelta = request.Quantity - item.Quantity;
        if (quantityDelta > 0)
        {
            if (product.StockCurrent < quantityDelta)
            {
                return Conflict(new { message = "Estoque insuficiente para aumentar quantidade do item." });
            }

            product.StockCurrent -= quantityDelta;
            dbContext.StockMovements.Add(new StockMovement
            {
                ProductId = product.Id,
                MovementType = "saida",
                Quantity = quantityDelta,
                Reason = $"Saida automatica por ajuste no pedido {order.Id}",
                CreatedAt = DateTime.UtcNow
            });
        }
        else if (quantityDelta < 0)
        {
            var returned = Math.Abs(quantityDelta);
            product.StockCurrent += returned;
            dbContext.StockMovements.Add(new StockMovement
            {
                ProductId = product.Id,
                MovementType = "entrada",
                Quantity = returned,
                Reason = $"Entrada automatica por ajuste no pedido {order.Id}",
                CreatedAt = DateTime.UtcNow
            });
        }

        item.Quantity = request.Quantity;
        item.LineTotal = item.UnitPrice * item.Quantity;

        order.UpdatedAt = DateTime.UtcNow;
        RecalculateTotals(order);

        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(order));
    }

    [HttpDelete("{id:int}/itens/{itemId:int}")]
    public async Task<ActionResult<OrderResponse>> DeleteItem(int id, int itemId, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (order is null)
        {
            return NotFound(new { message = "Pedido nao encontrado." });
        }

        var item = order.Items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return NotFound(new { message = "Item do pedido nao encontrado." });
        }

        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId, ct);
        if (product is not null)
        {
            product.StockCurrent += item.Quantity;
            dbContext.StockMovements.Add(new StockMovement
            {
                ProductId = product.Id,
                MovementType = "entrada",
                Quantity = item.Quantity,
                Reason = $"Entrada automatica por exclusao de item no pedido {order.Id}",
                CreatedAt = DateTime.UtcNow
            });
        }

        dbContext.OrderItems.Remove(item);

        order.UpdatedAt = DateTime.UtcNow;
        RecalculateTotals(order, order.Items.Where(x => x.Id != item.Id));

        await dbContext.SaveChangesAsync(ct);

        return Ok(ToResponse(order));
    }

    private static void RecalculateTotals(Order order)
    {
        RecalculateTotals(order, order.Items);
    }

    private static void RecalculateTotals(Order order, IEnumerable<OrderItem> items)
    {
        var subtotal = items.Sum(x => x.LineTotal);
        order.Subtotal = subtotal;
        order.Total = Math.Max(0, subtotal - order.DiscountTotal);
    }

    private static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            Subtotal = order.Subtotal,
            DiscountTotal = order.DiscountTotal,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Note = order.Note,
            Items = order.Items
                .OrderBy(x => x.Id)
                .Select(x => new OrderItemResponse
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    LineTotal = x.LineTotal
                })
                .ToList()
        };
    }
}
