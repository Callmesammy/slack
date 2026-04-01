using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.JoinChannel;

public sealed record JoinChannelCommand(Guid WorkspaceId, Guid ChannelId, Guid UserId)
    : IRequest<Result<JoinChannelResponse>>;

public sealed record JoinChannelResponse(Guid ChannelId);

