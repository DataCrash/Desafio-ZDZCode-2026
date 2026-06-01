using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class UpdateCategoryRequest
{
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
