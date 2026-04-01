using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Channels;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("channels");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.WorkspaceId).HasColumnName("workspace_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(80);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Topic).HasColumnName("topic");
        builder.Property(x => x.Purpose).HasColumnName("purpose");
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.IsArchived).HasColumnName("is_archived").IsRequired();
        builder.Property(x => x.ArchivedAt).HasColumnName("archived_at");
        builder.Property(x => x.ArchivedBy).HasColumnName("archived_by");

        builder.Property(x => x.MemberCount).HasColumnName("member_count").IsRequired();
        builder.Property(x => x.LastMessageAt).HasColumnName("last_message_at");

        builder.Property(x => x.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.WorkspaceId)
            .HasDatabaseName("channels_workspace_idx")
            .HasFilter("is_deleted = FALSE AND is_archived = FALSE");

        builder.HasIndex(x => new { x.WorkspaceId, x.Type })
            .HasDatabaseName("channels_type_idx");

        builder.HasIndex(x => new { x.WorkspaceId, x.LastMessageAt })
            .HasDatabaseName("channels_last_message_idx");

        builder.HasIndex(x => new { x.WorkspaceId, x.Name })
            .HasDatabaseName("channels_name_idx")
            .IsUnique()
            .HasFilter("type IN ('public', 'private') AND is_deleted = FALSE");
    }
}

