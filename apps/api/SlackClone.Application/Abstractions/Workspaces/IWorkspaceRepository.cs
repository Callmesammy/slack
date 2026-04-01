using SlackClone.Domain.Workspaces;

namespace SlackClone.Application.Abstractions.Workspaces;

public interface IWorkspaceRepository
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);
    Task AddAsync(Workspace workspace, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

