using SlackClone.Domain.Users;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Domain.Channels;

public sealed class ChannelMember
{
    private ChannelMember() { }

    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public Channel? Channel { get; private set; }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid WorkspaceId { get; private set; }
    public Workspace? Workspace { get; private set; }

    public string Role { get; private set; } = "member";
    public Guid? LastReadMessageId { get; private set; }
    public DateTimeOffset? LastReadAt { get; private set; }
    public string NotificationPref { get; private set; } = "all";
    public bool IsMuted { get; private set; }
    public bool IsStarred { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? LeftAt { get; private set; }

    public static ChannelMember Join(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        DateTimeOffset now,
        string role = "member")
    {
        return new ChannelMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            ChannelId = channelId,
            UserId = userId,
            Role = string.IsNullOrWhiteSpace(role) ? "member" : role,
            JoinedAt = now,
            LeftAt = null,
            NotificationPref = "all",
            IsMuted = false,
            IsStarred = false
        };
    }

    public void Leave(DateTimeOffset now)
    {
        LeftAt = now;
        LastReadAt ??= now;
    }
}
