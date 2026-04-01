using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Workspaces.CreateWorkspace;

public sealed record CreateWorkspaceCommand(
    Guid OwnerUserId,
    string Name,
    string? Description)
    : IRequest<Result<CreateWorkspaceResponse>>;

public sealed record CreateWorkspaceResponse(Guid WorkspaceId, string Slug);

