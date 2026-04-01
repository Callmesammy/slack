using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.ListChannelMessages;

public sealed record ListChannelMessagesQuery(Guid WorkspaceId, Guid ChannelId, Guid UserId, int Limit)
    : IRequest<Result<IReadOnlyList<ChannelMessageDto>>>;

public sealed record ChannelMessageDto(
    Guid Id,
    Guid SenderId,
    string SenderEmail,
    string? SenderName,
    string Content,
    string ContentFormat,
    string Type,
    Guid? ThreadParentId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<MessageReactionDto> Reactions);

public sealed record MessageReactionDto(
    string Emoji,
    int Count,
    bool ReactedByMe);
