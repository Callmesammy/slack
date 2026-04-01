namespace SlackClone.Domain.Users;

public sealed class User
{
    private User() { }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? GoogleId { get; private set; }
    public string Timezone { get; private set; } = "UTC";
    public string? StatusText { get; private set; }
    public string? StatusEmoji { get; private set; }
    public DateTimeOffset? StatusExpiresAt { get; private set; }
    public bool IsBot { get; private set; }
    public bool IsDeactivated { get; private set; }
    public DateTimeOffset? LastSeenAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static User CreateFromGoogle(
        string email,
        string name,
        string googleId,
        string? avatarUrl,
        string timezone,
        DateTimeOffset now)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name,
            GoogleId = googleId,
            AvatarUrl = avatarUrl,
            Timezone = timezone,
            IsBot = false,
            IsDeactivated = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateFromGoogle(string name, string? avatarUrl, DateTimeOffset now)
    {
        Name = name;
        AvatarUrl = avatarUrl;
        UpdatedAt = now;
    }
}
