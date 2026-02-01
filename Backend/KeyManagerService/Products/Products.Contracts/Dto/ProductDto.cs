namespace Products.Contracts.Dto;

public record ProductDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;
}