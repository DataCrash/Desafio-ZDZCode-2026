using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Domain;

public sealed class Tag
{
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}
