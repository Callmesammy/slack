using SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

namespace SlackClone.Application.Abstractions.Workspaces;

public interface IWorkspaceQueries
{
    Task<IReadOnlyList<UserWorkspaceDto>> GetUserWorkspacesAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<WorkspaceBySlugDto?> GetWorkspaceBySlugAsync(
        string slug,
        CancellationToken cancellationToken);
}

