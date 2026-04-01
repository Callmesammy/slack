using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackClone.Domain.Users;

namespace SlackClone.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(80);
        builder.Property(x => x.AvatarUrl).HasColumnName("avatar_url");
        builder.Property(x => x.GoogleId).HasColumnName("google_id").HasMaxLength(255);
        builder.Property(x => x.Timezone).HasColumnName("timezone").HasMaxLength(50).IsRequired();
        builder.Property(x => x.StatusText).HasColumnName("status_text").HasMaxLength(100);
        builder.Property(x => x.StatusEmoji).HasColumnName("status_emoji").HasMaxLength(50);
        builder.Property(x => x.StatusExpiresAt).HasColumnName("status_expires_at");
        builder.Property(x => x.IsBot).HasColumnName("is_bot").IsRequired();
        builder.Property(x => x.IsDeactivated).HasColumnName("is_deactivated").IsRequired();
        builder.Property(x => x.LastSeenAt).HasColumnName("last_seen_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(x => x.GoogleId).HasDatabaseName("users_google_id_idx");
        builder.HasIndex(x => x.LastSeenAt).HasDatabaseName("users_last_seen_idx");
        builder.HasIndex(x => x.Email).HasDatabaseName("users_email_idx").IsUnique();
    }
}

