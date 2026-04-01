using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.RemoveReaction;

public sealed record RemoveReactionCommand(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid MessageId,
    Guid UserId,
    string Emoji)
    : IRequest<Result<RemoveReactionResponse>>;

public sealed record RemoveReactionResponse(Guid MessageId, string Emoji, int Count);
