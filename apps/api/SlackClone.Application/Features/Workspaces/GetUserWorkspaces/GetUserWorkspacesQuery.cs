using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

public sealed record GetUserWorkspacesQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<UserWorkspaceDto>>>;

public sealed record UserWorkspaceDto(
    Guid Id,
    string Name,
    string Slug,
    string Role,
    string Plan);

public sealed record WorkspaceBySlugDto(
    Guid Id,
    string Name,
    string Slug);
