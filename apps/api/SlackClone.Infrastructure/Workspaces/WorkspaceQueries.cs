using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Features.Workspaces.GetUserWorkspaces;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Workspaces;

public sealed class WorkspaceQueries(SlackCloneDbContext dbContext) : IWorkspaceQueries
{
    public async Task<IReadOnlyList<UserWorkspaceDto>> GetUserWorkspacesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.IsActive)
            .Join(
                dbContext.Workspaces.AsNoTracking().Where(w => !w.IsDeleted),
                m => m.WorkspaceId,
                w => w.Id,
                (m, w) => new UserWorkspaceDto(
                    w.Id,
                    w.Name,
                    w.Slug,
                    m.Role.ToString().ToLowerInvariant(),
                    w.Plan.ToString().ToLowerInvariant()))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<WorkspaceBySlugDto?> GetWorkspaceBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        var normalized = slug.Trim().ToLowerInvariant();
        return dbContext.Workspaces
            .AsNoTracking()
            .Where(w => !w.IsDeleted && w.Slug.ToLower() == normalized)
            .Select(w => new WorkspaceBySlugDto(w.Id, w.Name, w.Slug))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
