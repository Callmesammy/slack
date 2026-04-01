using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Domain.Workspaces;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Workspaces;

public sealed class WorkspaceRepository(SlackCloneDbContext dbContext) : IWorkspaceRepository
{
    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        return dbContext.Workspaces.AsNoTracking().AnyAsync(
            w => !w.IsDeleted && w.Slug.ToLower() == normalized,
            cancellationToken);
    }

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        dbContext.Workspaces.Add(workspace);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

