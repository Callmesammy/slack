using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("workspaces");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Domain).HasColumnName("domain").HasMaxLength(253);
        builder.Property(x => x.LogoUrl).HasColumnName("logo_url");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Plan).HasColumnName("plan").IsRequired();
        builder.Property(x => x.PlanExpiresAt).HasColumnName("plan_expires_at");
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OwnerId).HasDatabaseName("workspaces_owner_idx");
        builder.HasIndex(x => x.Plan).HasDatabaseName("workspaces_plan_idx");
        builder.HasIndex(x => x.Slug)
            .HasDatabaseName("workspaces_slug_idx")
            .IsUnique();
    }
}

