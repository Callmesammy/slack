using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Domain.Messages;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Messages;

public sealed class MessageReactionRepository(SlackCloneDbContext dbContext) : IMessageReactionRepository
{
    public Task<MessageReaction?> GetAsync(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        Guid userId,
        string emoji,
        CancellationToken cancellationToken)
    {
        var normalizedEmoji = emoji.Trim();
        return dbContext.MessageReactions.FirstOrDefaultAsync(
            r => r.WorkspaceId == workspaceId
                 && r.ChannelId == channelId
                 && r.MessageId == messageId
                 && r.UserId == userId
                 && r.Emoji == normalizedEmoji,
            cancellationToken);
    }

    public Task AddAsync(MessageReaction reaction, CancellationToken cancellationToken)
    {
        dbContext.MessageReactions.Add(reaction);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(MessageReaction reaction, CancellationToken cancellationToken)
    {
        dbContext.MessageReactions.Remove(reaction);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

