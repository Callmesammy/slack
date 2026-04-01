using MediatR;
using SlackClone.Application.Common;
using SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

namespace SlackClone.Application.Features.Workspaces.GetWorkspaceBySlug;

public sealed record GetWorkspaceBySlugQuery(string Slug)
    : IRequest<Result<WorkspaceBySlugDto>>;

