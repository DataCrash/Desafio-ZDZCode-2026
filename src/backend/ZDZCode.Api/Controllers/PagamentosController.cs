using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/pagamentos")]
public sealed class PagamentosController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentResponse>>> GetAll(CancellationToken ct)
    {
        var payments = await dbContext.Payments
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => ToResponse(x))
            .ToListAsync(ct);

        return Ok(payments);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create(CreatePaymentRequest request, CancellationToken ct)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, ct);
        if (order is null)
        {
            return BadRequest(new { message = "Pedido invalido para pagamento." });
        }

        var alreadyExists = await dbContext.Payments.AnyAsync(x => x.OrderId == request.OrderId, ct);
        if (alreadyExists)
        {
            return Conflict(new { message = "Pedido ja possui pagamento vinculado." });
        }

        if (request.Value != order.Total)
        {
            return Conflict(new { message = "Valor do pagamento deve ser igual ao total do pedido." });
        }

        var payment = new Payment
        {
            OrderId = request.OrderId,
            Method = request.Method.Trim(),
            Status = request.Status.Trim(),
            Value = request.Value,
            TransactionReference = request.TransactionReference?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), new { id = payment.Id }, ToResponse(payment));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PaymentResponse>> Update(int id, UpdatePaymentRequest request, CancellationToken ct)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (payment is null)
        {
            return NotFound();
        }

        payment.Status = request.Status.Trim();
        payment.TransactionReference = request.TransactionReference?.Trim();

        await dbContext.SaveChangesAsync(ct);
        return Ok(ToResponse(payment));
    }

    private static PaymentResponse ToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Method = payment.Method,
            Status = payment.Status,
            Value = payment.Value,
            TransactionReference = payment.TransactionReference,
            CreatedAt = payment.CreatedAt
        };
    }
}
