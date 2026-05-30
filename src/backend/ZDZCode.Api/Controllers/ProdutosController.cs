using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public sealed class ProdutosController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken ct)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Id)
            .Select(x => new ProductResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                CategoryId = x.CategoryId,
                Category = x.Category == null
                    ? null
                    : new CategoryResponse
                    {
                        Id = x.Category.Id,
                        Name = x.Category.Name,
                        Description = x.Category.Description
                    }
            })
            .ToListAsync(ct);

        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var categoryExists = await dbContext.Categories.AnyAsync(x => x.Id == request.CategoryId, ct);
        if (!categoryExists)
        {
            return BadRequest(new { message = "Categoria inválida para o produto informado." });
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);

        var created = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstAsync(x => x.Id == product.Id, ct);

        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, new ProductResponse
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Price = created.Price,
            CategoryId = created.CategoryId,
            Category = created.Category == null
                ? null
                : new CategoryResponse
                {
                    Id = created.Category.Id,
                    Name = created.Category.Name,
                    Description = created.Category.Description
                }
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponse>> Update(int id, UpdateProductRequest request, CancellationToken ct)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (product is null)
        {
            return NotFound();
        }

        var categoryExists = await dbContext.Categories.AnyAsync(x => x.Id == request.CategoryId, ct);
        if (!categoryExists)
        {
            return BadRequest(new { message = "Categoria inválida para o produto informado." });
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync(ct);

        var updated = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstAsync(x => x.Id == product.Id, ct);

        return Ok(new ProductResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            Price = updated.Price,
            CategoryId = updated.CategoryId,
            Category = updated.Category == null
                ? null
                : new CategoryResponse
                {
                    Id = updated.Category.Id,
                    Name = updated.Category.Name,
                    Description = updated.Category.Description
                }
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (product is null)
        {
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}
