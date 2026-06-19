using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Contracts;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

namespace ZDZCode.Api.Controllers;

[ApiController]
[Route("api/tags")]
public sealed class TagsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagResponse>>> GetAll(CancellationToken ct)
    {
        var tags = await dbContext.Tags
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new TagResponse
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);

        return Ok(tags);
    }

    [HttpPost]
    public async Task<ActionResult<TagResponse>> Create(CreateTagRequest request, CancellationToken ct)
    {
        var tag = new Tag
        {
            Name = request.Name.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), new { id = tag.Id }, new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            IsActive = tag.IsActive
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagResponse>> Update(int id, UpdateTagRequest request, CancellationToken ct)
    {
        var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (tag is null)
        {
            return NotFound();
        }

        tag.Name = request.Name.Trim();
        tag.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(ct);

        return Ok(new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            IsActive = tag.IsActive
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var tag = await dbContext.Tags
            .Include(x => x.ProductTags)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (tag is null)
        {
            return NotFound();
        }

        if (tag.ProductTags.Count > 0)
        {
            return Conflict(new { message = "Nao e possivel excluir tag vinculada a produtos." });
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{tagId:int}/produtos/{productId:int}")]
    public async Task<ActionResult> LinkProduct(int tagId, int productId, CancellationToken ct)
    {
        var tagExists = await dbContext.Tags.AnyAsync(x => x.Id == tagId, ct);
        if (!tagExists)
        {
            return NotFound(new { message = "Tag nao encontrada." });
        }

        var productExists = await dbContext.Products.AnyAsync(x => x.Id == productId, ct);
        if (!productExists)
        {
            return NotFound(new { message = "Produto nao encontrado." });
        }

        var linkExists = await dbContext.ProductTags.AnyAsync(x => x.TagId == tagId && x.ProductId == productId, ct);
        if (linkExists)
        {
            return Conflict(new { message = "Vinculo produto/tag ja existe." });
        }

        dbContext.ProductTags.Add(new ProductTag
        {
            TagId = tagId,
            ProductId = productId
        });

        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{tagId:int}/produtos/{productId:int}")]
    public async Task<ActionResult> UnlinkProduct(int tagId, int productId, CancellationToken ct)
    {
        var link = await dbContext.ProductTags.FirstOrDefaultAsync(x => x.TagId == tagId && x.ProductId == productId, ct);
        if (link is null)
        {
            return NotFound();
        }

        dbContext.ProductTags.Remove(link);
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}
