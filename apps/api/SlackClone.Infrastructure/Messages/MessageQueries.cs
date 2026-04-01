using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Features.Messages.ListChannelMessages;
using SlackClone.Application.Features.Messages.ListChannelMessagesPage;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Messages;

public sealed class MessageQueries(SlackCloneDbContext dbContext) : IMessageQueries
{
    public async Task<IReadOnlyList<ChannelMessageDto>> ListChannelMessagesAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        int limit,
        CancellationToken cancellationToken)
    {
        var messages = await (
                from message in dbContext.Messages.AsNoTracking()
                join user in dbContext.Users.AsNoTracking() on message.SenderId equals user.Id
                where message.WorkspaceId == workspaceId
                      && message.ChannelId == channelId
                      && !message.IsDeleted
                orderby message.CreatedAt descending
                select new ChannelMessageDto(
                    message.Id,
                    message.SenderId,
                    user.Email,
                    user.Name,
                    message.Content,
                    message.ContentFormat,
                    message.Type.ToString().ToLowerInvariant(),
                    message.ThreadParentId,
                    message.CreatedAt,
                    Array.Empty<MessageReactionDto>()))
            .Take(limit)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return messages;
        }

        var messageIds = messages.Select(m => m.Id).ToArray();

        var reactions = await dbContext.MessageReactions
            .AsNoTracking()
            .Where(r => r.WorkspaceId == workspaceId && r.ChannelId == channelId && messageIds.Contains(r.MessageId))
            .GroupBy(r => new { r.MessageId, r.Emoji })
            .Select(g => new
            {
                g.Key.MessageId,
                g.Key.Emoji,
                Count = g.Count(),
                ReactedByMe = g.Any(r => r.UserId == userId)
            })
            .ToListAsync(cancellationToken);

        var reactionsByMessageId = reactions
            .GroupBy(r => r.MessageId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<MessageReactionDto>)g
                    .OrderByDescending(x => x.Count)
                    .ThenBy(x => x.Emoji)
                    .Select(x => new MessageReactionDto(x.Emoji, x.Count, x.ReactedByMe))
                    .ToList());

        return messages
            .Select(m => reactionsByMessageId.TryGetValue(m.Id, out var rs)
                ? m with { Reactions = rs }
                : m)
            .ToList();
    }

    public async Task<ChannelMessagePageDto> ListChannelMessagesPageAsync(
        Guid workspaceId,
        Guid channelId,
        int limit,
        Guid? cursorMessageId,
        CancellationToken cancellationToken)
    {
        var safeLimit = limit is > 0 and <= 100 ? limit : 50;

        DateTimeOffset? cursorCreatedAt = null;
        Guid? cursorId = null;

        if (cursorMessageId is not null)
        {
            var cursorRow = await dbContext.Messages
                .AsNoTracking()
                .Where(m => m.WorkspaceId == workspaceId && m.ChannelId == channelId && m.Id == cursorMessageId.Value && !m.IsDeleted)
                .Select(m => new { m.CreatedAt, m.Id })
                .FirstOrDefaultAsync(cancellationToken);

            if (cursorRow is not null)
            {
                cursorCreatedAt = cursorRow.CreatedAt;
                cursorId = cursorRow.Id;
            }
        }

        var query = dbContext.Messages
            .AsNoTracking()
            .Where(m => m.WorkspaceId == workspaceId && m.ChannelId == channelId && !m.IsDeleted);

        if (cursorCreatedAt is not null && cursorId is not null)
        {
            query = query.Where(m =>
                m.CreatedAt < cursorCreatedAt.Value ||
                (m.CreatedAt == cursorCreatedAt.Value && m.Id.CompareTo(cursorId.Value) < 0));
        }

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(safeLimit)
            .Select(m => new ChannelMessageItemDto(
                m.Id,
                m.SenderId,
                m.Content,
                m.ContentFormat,
                m.Type.ToString().ToLowerInvariant(),
                m.ThreadParentId,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        Guid? nextCursor = items.Count == safeLimit ? items[^1].Id : null;
        return new ChannelMessagePageDto(items, nextCursor);
    }

    public async Task<int> GetReactionCountAsync(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        string emoji,
        CancellationToken cancellationToken)
    {
        var normalizedEmoji = emoji.Trim();
        return await dbContext.MessageReactions
            .AsNoTracking()
            .Where(r => r.WorkspaceId == workspaceId
                        && r.ChannelId == channelId
                        && r.MessageId == messageId
                        && r.Emoji == normalizedEmoji)
            .CountAsync(cancellationToken);
    }
}
