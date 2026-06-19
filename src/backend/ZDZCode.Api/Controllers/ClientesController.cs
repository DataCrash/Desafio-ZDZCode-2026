using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll(CancellationToken ct)
    {
        var customers = await dbContext.Customers
            .AsNoTracking()
            .Include(x => x.DeliveryAddress)
            .OrderBy(x => x.Id)
            .Select(x => ToResponse(x))
            .ToListAsync(ct);

        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken ct)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim();
        var emailInUse = await dbContext.Customers.AnyAsync(x => x.Email == normalizedEmail, ct);
        if (emailInUse)
        {
            return Conflict(new { message = "E-mail ja cadastrado para outro cliente." });
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            Phone = request.Phone?.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        if (request.DeliveryAddress is not null)
        {
            customer.DeliveryAddress = new DeliveryAddress
            {
                Street = request.DeliveryAddress.Street.Trim(),
                Number = request.DeliveryAddress.Number.Trim(),
                District = request.DeliveryAddress.District?.Trim(),
                City = request.DeliveryAddress.City.Trim(),
                State = request.DeliveryAddress.State.Trim(),
                ZipCode = request.DeliveryAddress.ZipCode.Trim(),
                Complement = request.DeliveryAddress.Complement?.Trim()
            };
        }

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(ct);

        var created = await dbContext.Customers
            .AsNoTracking()
            .Include(x => x.DeliveryAddress)
            .FirstAsync(x => x.Id == customer.Id, ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> Update(int id, UpdateCustomerRequest request, CancellationToken ct)
    {
        var customer = await dbContext.Customers
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (customer is null)
        {
            return NotFound();
        }

        var normalizedEmail = request.Email.Trim();
        var emailInUse = await dbContext.Customers.AnyAsync(x => x.Id != id && x.Email == normalizedEmail, ct);
        if (emailInUse)
        {
            return Conflict(new { message = "E-mail ja cadastrado para outro cliente." });
        }

        customer.Name = request.Name.Trim();
        customer.Email = normalizedEmail;
        customer.Phone = request.Phone?.Trim();
        customer.IsActive = request.IsActive;

        if (request.DeliveryAddress is null)
        {
            if (customer.DeliveryAddress is not null)
            {
                dbContext.DeliveryAddresses.Remove(customer.DeliveryAddress);
            }
        }
        else
        {
            if (customer.DeliveryAddress is null)
            {
                customer.DeliveryAddress = new DeliveryAddress { CustomerId = customer.Id };
            }

            customer.DeliveryAddress.Street = request.DeliveryAddress.Street.Trim();
            customer.DeliveryAddress.Number = request.DeliveryAddress.Number.Trim();
            customer.DeliveryAddress.District = request.DeliveryAddress.District?.Trim();
            customer.DeliveryAddress.City = request.DeliveryAddress.City.Trim();
            customer.DeliveryAddress.State = request.DeliveryAddress.State.Trim();
            customer.DeliveryAddress.ZipCode = request.DeliveryAddress.ZipCode.Trim();
            customer.DeliveryAddress.Complement = request.DeliveryAddress.Complement?.Trim();
        }

        await dbContext.SaveChangesAsync(ct);

        var updated = await dbContext.Customers
            .AsNoTracking()
            .Include(x => x.DeliveryAddress)
            .FirstAsync(x => x.Id == id, ct);

        return Ok(ToResponse(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var customer = await dbContext.Customers
            .Include(x => x.Orders)
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (customer is null)
        {
            return NotFound();
        }

        if (customer.Orders.Count > 0)
        {
            return Conflict(new { message = "Nao e possivel excluir cliente com pedidos vinculados." });
        }

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    private static CustomerResponse ToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            DeliveryAddress = customer.DeliveryAddress is null
                ? null
                : new DeliveryAddressResponse
                {
                    Id = customer.DeliveryAddress.Id,
                    CustomerId = customer.DeliveryAddress.CustomerId,
                    Street = customer.DeliveryAddress.Street,
                    Number = customer.DeliveryAddress.Number,
                    District = customer.DeliveryAddress.District,
                    City = customer.DeliveryAddress.City,
                    State = customer.DeliveryAddress.State,
                    ZipCode = customer.DeliveryAddress.ZipCode,
                    Complement = customer.DeliveryAddress.Complement
                }
        };
    }
}
