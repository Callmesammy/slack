using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.SendMessage;

public sealed record SendMessageCommand(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid SenderId,
    string Content,
    string ContentFormat)
    : IRequest<Result<SendMessageResponse>>;

public sealed record SendMessageResponse(
    Guid MessageId,
    Guid WorkspaceId,
    Guid ChannelId,
    Guid SenderId,
    DateTimeOffset CreatedAt,
    string Content,
    string ContentFormat);
