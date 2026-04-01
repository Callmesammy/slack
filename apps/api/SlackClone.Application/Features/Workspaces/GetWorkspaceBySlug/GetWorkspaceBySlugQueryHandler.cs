using MediatR;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

namespace SlackClone.Application.Features.Workspaces.GetWorkspaceBySlug;

public sealed class GetWorkspaceBySlugQueryHandler(IWorkspaceQueries workspaceQueries)
    : IRequestHandler<GetWorkspaceBySlugQuery, Result<WorkspaceBySlugDto>>
{
    public async Task<Result<WorkspaceBySlugDto>> Handle(
        GetWorkspaceBySlugQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            return Result<WorkspaceBySlugDto>.Failure("VALIDATION_ERROR", "Slug is required.");
        }

        var workspace = await workspaceQueries.GetWorkspaceBySlugAsync(
            request.Slug,
            cancellationToken);

        if (workspace is null)
        {
            return Result<WorkspaceBySlugDto>.Failure("WORKSPACE_NOT_FOUND", "Workspace not found.");
        }

        return Result<WorkspaceBySlugDto>.Success(workspace);
    }
}

