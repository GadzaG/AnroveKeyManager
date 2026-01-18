using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Products;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Postgres.Configurations;

public static class Index
{
    public const string TITLE = "ix_lessons_title";
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.OwnsOne(x => x.Title, b =>
        {
            b.Property(x => x.Value).HasColumnName("title");

            b.HasIndex(x => x.Value).IsUnique().HasDatabaseName(Index.TITLE);
        });

        builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");

        builder.Property(x => x.Description)
            .HasConversion(
                v => v.Value,
                v => Description.Create(v).Value)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(l => l.IsDeleted)
            .HasDefaultValue(false)
            .HasColumnName("is_deleted");

        builder.Property(l => l.DeletedAt)
            .IsRequired(false)
            .HasColumnName("deleted_at");

        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .HasColumnName("created_at");

        builder.Property(l => l.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .HasColumnName("updated_at");

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}