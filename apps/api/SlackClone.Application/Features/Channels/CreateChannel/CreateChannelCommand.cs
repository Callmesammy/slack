using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.CreateChannel;

public sealed record CreateChannelCommand(
    Guid WorkspaceId,
    Guid CreatorUserId,
    string Name,
    string? Description,
    string Type)
    : IRequest<Result<CreateChannelResponse>>;

public sealed record CreateChannelResponse(Guid ChannelId);

