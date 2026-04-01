using SlackClone.Domain.Messages;

namespace SlackClone.Application.Abstractions.Messages;

public interface IMessageRepository
{
    Task<Message?> GetAsync(Guid workspaceId, Guid channelId, Guid messageId, CancellationToken cancellationToken);
    Task AddAsync(Message message, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
