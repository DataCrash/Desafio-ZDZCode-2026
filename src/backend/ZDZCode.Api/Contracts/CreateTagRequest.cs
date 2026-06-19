using System.ComponentModel.DataAnnotations;

namespace ZDZCode.Api.Contracts;

public sealed class CreateTagRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
