using SlackClone.Domain.Messages;

namespace SlackClone.Application.Abstractions.Messages;

public interface IMessageReactionRepository
{
    Task<MessageReaction?> GetAsync(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        Guid userId,
        string emoji,
        CancellationToken cancellationToken);

    Task AddAsync(MessageReaction reaction, CancellationToken cancellationToken);

    Task RemoveAsync(MessageReaction reaction, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

