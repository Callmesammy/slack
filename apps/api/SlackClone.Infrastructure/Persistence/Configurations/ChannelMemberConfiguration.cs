using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Channels;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class ChannelMemberConfiguration : IEntityTypeConfiguration<ChannelMember>
{
    public void Configure(EntityTypeBuilder<ChannelMember> builder)
    {
        builder.ToTable("channel_members");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.ChannelId).HasColumnName("channel_id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.WorkspaceId).HasColumnName("workspace_id").IsRequired();

        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
        builder.Property(x => x.LastReadMessageId).HasColumnName("last_read_message_id");
        builder.Property(x => x.LastReadAt).HasColumnName("last_read_at");
        builder.Property(x => x.NotificationPref).HasColumnName("notification_pref").HasMaxLength(20).IsRequired();
        builder.Property(x => x.IsMuted).HasColumnName("is_muted").IsRequired();
        builder.Property(x => x.IsStarred).HasColumnName("is_starred").IsRequired();
        builder.Property(x => x.JoinedAt).HasColumnName("joined_at").IsRequired();
        builder.Property(x => x.LeftAt).HasColumnName("left_at");

        builder.HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ChannelId, x.UserId })
            .HasDatabaseName("channel_members_unique_idx")
            .IsUnique()
            .HasFilter("left_at IS NULL");

        builder.HasIndex(x => new { x.UserId, x.WorkspaceId })
            .HasDatabaseName("channel_members_user_idx")
            .HasFilter("left_at IS NULL");

        builder.HasIndex(x => x.ChannelId)
            .HasDatabaseName("channel_members_channel_idx")
            .HasFilter("left_at IS NULL");
    }
}

