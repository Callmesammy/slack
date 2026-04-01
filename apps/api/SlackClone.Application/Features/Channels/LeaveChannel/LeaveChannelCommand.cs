using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.LeaveChannel;

public sealed record LeaveChannelCommand(Guid WorkspaceId, Guid ChannelId, Guid UserId)
    : IRequest<Result<LeaveChannelResponse>>;

public sealed record LeaveChannelResponse(Guid ChannelId);

