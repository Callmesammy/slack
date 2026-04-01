using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Domain.Workspaces;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Workspaces;

public sealed class WorkspaceMemberRepository(SlackCloneDbContext dbContext) : IWorkspaceMemberRepository
{
    public Task AddAsync(WorkspaceMember member, CancellationToken cancellationToken)
    {
        dbContext.WorkspaceMembers.Add(member);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

