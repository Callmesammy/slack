using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.EditMessage;

public sealed record EditMessageCommand(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid MessageId,
    Guid EditorUserId,
    string Content,
    string ContentFormat)
    : IRequest<Result<EditMessageResponse>>;

public sealed record EditMessageResponse(
    Guid MessageId,
    Guid WorkspaceId,
    Guid ChannelId,
    Guid EditorUserId,
    DateTimeOffset EditedAt,
    string Content,
    string ContentFormat);
