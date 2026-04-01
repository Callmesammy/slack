using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Workspaces;

public sealed class WorkspaceMembershipService(SlackCloneDbContext dbContext) : IWorkspaceMembershipService
{
    public Task<bool> IsMemberAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.WorkspaceMembers
            .AsNoTracking()
            .AnyAsync(
                m => m.WorkspaceId == workspaceId && m.UserId == userId && m.IsActive,
                cancellationToken);
    }
}

