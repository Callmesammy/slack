using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.ListChannelMessagesPage;

public sealed record ListChannelMessagesPageQuery(
    Guid WorkspaceId,
    Guid ChannelId,
    Guid UserId,
    int Limit,
    Guid? CursorMessageId)
    : IRequest<Result<ChannelMessagePageDto>>;

public sealed record ChannelMessagePageDto(
    IReadOnlyList<ChannelMessageItemDto> Items,
    Guid? NextCursorMessageId);

public sealed record ChannelMessageItemDto(
    Guid Id,
    Guid SenderId,
    string Content,
    string ContentFormat,
    string Type,
    Guid? ThreadParentId,
    DateTimeOffset CreatedAt);

