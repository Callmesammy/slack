namespace SlackClone.Application.Abstractions.Workspaces;

public interface IWorkspaceMembershipService
{
    Task<bool> IsMemberAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
}

