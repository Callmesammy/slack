using SlackClone.Domain.Workspaces;

namespace SlackClone.Application.Abstractions.Workspaces;

public interface IWorkspaceMemberRepository
{
    Task AddAsync(WorkspaceMember member, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

