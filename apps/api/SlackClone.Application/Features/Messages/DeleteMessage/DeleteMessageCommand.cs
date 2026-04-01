using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.DeleteMessage;

public sealed record DeleteMessageCommand(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid MessageId,
    Guid DeleterUserId)
    : IRequest<Result<DeleteMessageResponse>>;

public sealed record DeleteMessageResponse(
    Guid MessageId,
    Guid WorkspaceId,
    Guid ChannelId,
    Guid DeleterUserId,
    DateTimeOffset DeletedAt);
