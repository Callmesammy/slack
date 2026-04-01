using SlackClone.Domain.Users;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Domain.Channels;

public sealed class Channel
{
    private Channel() { }

    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Workspace? Workspace { get; private set; }

    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Topic { get; private set; }
    public string? Purpose { get; private set; }
    public ChannelType Type { get; private set; } = ChannelType.Public;

    public bool IsArchived { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }
    public Guid? ArchivedBy { get; private set; }

    public int MemberCount { get; private set; }
    public DateTimeOffset? LastMessageAt { get; private set; }

    public Guid CreatedBy { get; private set; }
    public User? CreatedByUser { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void IncrementMemberCount(DateTimeOffset now)
    {
        MemberCount++;
        UpdatedAt = now;
    }

    public void DecrementMemberCount(DateTimeOffset now)
    {
        if (MemberCount > 0)
        {
            MemberCount--;
        }

        UpdatedAt = now;
    }

    public static Channel CreatePublicChannel(
        Guid workspaceId,
        string name,
        Guid createdBy,
        DateTimeOffset now,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        return new Channel
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            Name = name.Trim(),
            Description = description,
            Type = ChannelType.Public,
            CreatedBy = createdBy,
            IsArchived = false,
            IsDeleted = false,
            MemberCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
