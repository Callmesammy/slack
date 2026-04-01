using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Domain.Messages;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Messages;

public sealed class MessageRepository(SlackCloneDbContext dbContext) : IMessageRepository
{
    public Task<Message?> GetAsync(Guid workspaceId, Guid channelId, Guid messageId, CancellationToken cancellationToken)
    {
        return dbContext.Messages.FirstOrDefaultAsync(
            m => m.Id == messageId
                 && m.WorkspaceId == workspaceId
                 && m.ChannelId == channelId,
            cancellationToken);
    }

    public Task AddAsync(Message message, CancellationToken cancellationToken)
    {
        dbContext.Messages.Add(message);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
