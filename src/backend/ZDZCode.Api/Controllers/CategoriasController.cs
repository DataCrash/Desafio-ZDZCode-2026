using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public sealed class CategoriasController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(CancellationToken ct)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CategoryResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync(ct);

        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(int id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();

        await dbContext.SaveChangesAsync(ct);

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var category = await dbContext.Categories
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (category is null)
        {
            return NotFound();
        }

        if (category.Products.Count > 0)
        {
            return Conflict(new
            {
                message = "Não é possível excluir uma categoria que possua produtos vinculados."
            });
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}
