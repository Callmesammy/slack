using MediatR;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

public sealed class GetUserWorkspacesQueryHandler(IWorkspaceQueries workspaceQueries)
    : IRequestHandler<GetUserWorkspacesQuery, Result<IReadOnlyList<UserWorkspaceDto>>>
{
    public async Task<Result<IReadOnlyList<UserWorkspaceDto>>> Handle(
        GetUserWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await workspaceQueries.GetUserWorkspacesAsync(request.UserId, cancellationToken);
        return Result<IReadOnlyList<UserWorkspaceDto>>.Success(list);
    }
}

