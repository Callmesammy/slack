using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Features.Channels.ListWorkspaceChannels;
using SlackClone.Domain.Channels;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Channels;

public sealed class ChannelQueries(SlackCloneDbContext dbContext) : IChannelQueries
{
    public async Task<IReadOnlyList<WorkspaceChannelDto>> ListWorkspaceChannelsAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query =
            from channel in dbContext.Channels.AsNoTracking()
            where channel.WorkspaceId == workspaceId && !channel.IsDeleted
            join membership in dbContext.ChannelMembers.AsNoTracking().Where(m => m.UserId == userId && m.LeftAt == null)
                on channel.Id equals membership.ChannelId into membershipJoin
            from membership in membershipJoin.DefaultIfEmpty()
            where channel.Type == ChannelType.Public || membership != null
            select channel;

        return await query
            .OrderByDescending(c => c.LastMessageAt)
            .ThenBy(c => c.Name)
            .Select(c => new WorkspaceChannelDto(
                c.Id,
                c.Name,
                c.Type.ToString().ToLowerInvariant(),
                c.IsArchived,
                c.MemberCount,
                c.LastMessageAt))
            .ToListAsync(cancellationToken);
    }
}
