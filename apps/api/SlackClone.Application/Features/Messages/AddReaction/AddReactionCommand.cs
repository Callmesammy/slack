using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.AddReaction;

public sealed record AddReactionCommand(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid MessageId,
    Guid UserId,
    string Emoji)
    : IRequest<Result<AddReactionResponse>>;

public sealed record AddReactionResponse(Guid MessageId, string Emoji, int Count);
