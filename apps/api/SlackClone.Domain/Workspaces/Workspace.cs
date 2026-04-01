using SlackClone.Domain.Users;

namespace SlackClone.Domain.Workspaces;

public sealed class Workspace
{
    private Workspace() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Domain { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Description { get; private set; }
    public PlanType Plan { get; private set; } = PlanType.Free;
    public DateTimeOffset? PlanExpiresAt { get; private set; }
    public Guid OwnerId { get; private set; }
    public User? Owner { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Workspace Create(
        string name,
        string slug,
        Guid ownerId,
        DateTimeOffset now,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug is required.", nameof(slug));

        return new Workspace
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = slug.Trim(),
            OwnerId = ownerId,
            Description = description,
            Plan = PlanType.Free,
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
