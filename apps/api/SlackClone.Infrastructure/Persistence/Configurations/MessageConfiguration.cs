using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Messages;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.WorkspaceId).HasColumnName("workspace_id").IsRequired();
        builder.Property(x => x.ChannelId).HasColumnName("channel_id").IsRequired();
        builder.Property(x => x.SenderId).HasColumnName("sender_id").IsRequired();

        builder.Property(x => x.Content).HasColumnName("content").IsRequired();
        builder.Property(x => x.ContentFormat).HasColumnName("content_format").HasMaxLength(10).IsRequired();
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ThreadParentId).HasColumnName("thread_parent_id");
        builder.Property(x => x.ReplyCount).HasColumnName("reply_count").IsRequired();
        builder.Property(x => x.LatestReplyAt).HasColumnName("latest_reply_at");
        builder.Property(x => x.LatestReplySenderIds).HasColumnName("latest_reply_sender_ids");

        builder.Property(x => x.IsPinned).HasColumnName("is_pinned").IsRequired();
        builder.Property(x => x.PinnedAt).HasColumnName("pinned_at");
        builder.Property(x => x.PinnedBy).HasColumnName("pinned_by");

        builder.Property(x => x.IsEdited).HasColumnName("is_edited").IsRequired();
        builder.Property(x => x.EditedAt).HasColumnName("edited_at");

        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.DeletedBy).HasColumnName("deleted_by");

        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne(x => x.ThreadParent)
            .WithMany()
            .HasForeignKey(x => x.ThreadParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ChannelId)
            .HasDatabaseName("messages_channel_idx")
            .HasFilter("is_deleted = FALSE");

        builder.HasIndex(x => x.ThreadParentId)
            .HasDatabaseName("messages_thread_idx")
            .HasFilter("thread_parent_id IS NOT NULL");

        builder.HasIndex(x => new { x.SenderId, x.WorkspaceId })
            .HasDatabaseName("messages_sender_idx");

        builder.HasIndex(x => x.ChannelId)
            .HasDatabaseName("messages_pinned_idx")
            .HasFilter("is_pinned = TRUE");

        builder.HasIndex(x => new { x.WorkspaceId, x.CreatedAt })
            .HasDatabaseName("messages_workspace_ts_idx")
            .HasFilter("is_deleted = FALSE");

        // FTS index (expression) not modeled in EF; added later via raw SQL migration if needed.
    }
}

