using SlackClone.Domain.Users;

namespace SlackClone.Domain.Workspaces;

public sealed class WorkspaceMember
{
    private WorkspaceMember() { }

    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Workspace? Workspace { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public WorkspaceRole Role { get; private set; } = WorkspaceRole.Member;
    public string? DisplayName { get; private set; }
    public string? Title { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? DeactivatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static WorkspaceMember CreateOwner(Guid workspaceId, Guid userId, DateTimeOffset now)
    {
        return new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = WorkspaceRole.Owner,
            IsActive = true,
            JoinedAt = now,
            UpdatedAt = now
        };
    }
}
