using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/estoque")]
public sealed class EstoqueController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("movimentacoes")]
    public async Task<ActionResult<IReadOnlyList<StockMovementResponse>>> GetMovements(CancellationToken ct)
    {
        var movements = await dbContext.StockMovements
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Select(x => new StockMovementResponse
            {
                Id = x.Id,
                ProductId = x.ProductId,
                MovementType = x.MovementType,
                Quantity = x.Quantity,
                Reason = x.Reason,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(movements);
    }

    [HttpPost("movimentacoes")]
    public async Task<ActionResult<StockMovementResponse>> CreateMovement(CreateStockMovementRequest request, CancellationToken ct)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (product is null)
        {
            return BadRequest(new { message = "Produto invalido para movimentacao de estoque." });
        }

        var movementType = request.MovementType.Trim().ToLowerInvariant();
        if (movementType is not ("entrada" or "saida"))
        {
            return BadRequest(new { message = "Tipo de movimentacao invalido. Use 'entrada' ou 'saida'." });
        }

        var signedQuantity = movementType == "entrada" ? request.Quantity : -request.Quantity;
        var nextStock = product.StockCurrent + signedQuantity;
        if (nextStock < 0)
        {
            return Conflict(new { message = "Nao e permitido estoque negativo." });
        }

        product.StockCurrent = nextStock;

        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            MovementType = movementType,
            Quantity = request.Quantity,
            Reason = request.Reason?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.StockMovements.Add(movement);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetMovements), new { id = movement.Id }, new StockMovementResponse
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            MovementType = movement.MovementType,
            Quantity = movement.Quantity,
            Reason = movement.Reason,
            CreatedAt = movement.CreatedAt
        });
    }
}
