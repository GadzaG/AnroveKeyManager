using CSharpFunctionalExtensions;
using Products.Domain.ValueObjects;
using Shared.Kernel;

namespace Products.Domain;

public sealed class Product : Entity<Guid>
{
    private Product()
    {
    }

    private Product(
        Guid id,
        Guid userId,
        Title title,
        Description description)
        : base(id)
    {
        UserId = userId;
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        IsPublic = false;
    }

    public Guid UserId { get; private set; }

    public Guid? LogoId { get; private set; }

    public Guid? BackgroundId { get; private set; }

    public Title Title { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public bool IsPublic { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public static Result<Product, Error> Create(Guid id, Guid userId, Title title, Description description)
    {
        return new Product(id, userId, title, description);
    }

    public void UpdateLogo(Guid? logoId)
    {
        LogoId = logoId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBackground(Guid? backgroundId)
    {
        BackgroundId = backgroundId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(Title title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }
}