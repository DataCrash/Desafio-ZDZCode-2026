namespace ZDZCode.Api.Contracts;

public sealed class TagResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
