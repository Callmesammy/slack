using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Domain.Channels;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Channels;

public sealed class ChannelAccessService(SlackCloneDbContext dbContext) : IChannelAccessService
{
    public async Task<ChannelAccessCheck> CheckAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId && c.Id == channelId && !c.IsDeleted)
            .Select(c => new { c.Type, c.IsArchived })
            .FirstOrDefaultAsync(cancellationToken);

        if (channel is null)
        {
            return new ChannelAccessCheck(Exists: false, CanRead: false, CanPost: false);
        }

        if (channel.Type == ChannelType.Public)
        {
            return new ChannelAccessCheck(
                Exists: true,
                CanRead: true,
                CanPost: !channel.IsArchived);
        }

        var isMember = await dbContext.ChannelMembers
            .AsNoTracking()
            .AnyAsync(
                m => m.WorkspaceId == workspaceId
                     && m.ChannelId == channelId
                     && m.UserId == userId
                     && m.LeftAt == null,
                cancellationToken);

        return new ChannelAccessCheck(
            Exists: true,
            CanRead: isMember,
            CanPost: isMember && !channel.IsArchived);
    }
}

