using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Messages;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class MessageMentionConfiguration : IEntityTypeConfiguration<MessageMention>
{
    public void Configure(EntityTypeBuilder<MessageMention> builder)
    {
        builder.ToTable("message_mentions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.MessageId).HasColumnName("message_id").IsRequired();
        builder.Property(x => x.WorkspaceId).HasColumnName("workspace_id").IsRequired();
        builder.Property(x => x.ChannelId).HasColumnName("channel_id").IsRequired();
        builder.Property(x => x.MentionedUserId).HasColumnName("mentioned_user_id");
        builder.Property(x => x.MentionType).HasColumnName("mention_type").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasOne(x => x.Message)
            .WithMany()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MentionedUser)
            .WithMany()
            .HasForeignKey(x => x.MentionedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.MentionedUserId, x.WorkspaceId })
            .HasDatabaseName("message_mentions_user_idx");

        builder.HasIndex(x => x.MessageId)
            .HasDatabaseName("message_mentions_message_idx");
    }
}

