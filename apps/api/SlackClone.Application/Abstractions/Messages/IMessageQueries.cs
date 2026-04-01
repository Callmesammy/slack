using SlackClone.Application.Features.Messages.ListChannelMessages;
using SlackClone.Application.Features.Messages.ListChannelMessagesPage;

namespace SlackClone.Application.Abstractions.Messages;

public interface IMessageQueries
{
    Task<IReadOnlyList<ChannelMessageDto>> ListChannelMessagesAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        int limit,
        CancellationToken cancellationToken);

    Task<ChannelMessagePageDto> ListChannelMessagesPageAsync(
        Guid workspaceId,
        Guid channelId,
        int limit,
        Guid? cursorMessageId,
        CancellationToken cancellationToken);

    Task<int> GetReactionCountAsync(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        string emoji,
        CancellationToken cancellationToken);
}
