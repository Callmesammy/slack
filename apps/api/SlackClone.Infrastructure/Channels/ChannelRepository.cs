using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Domain.Channels;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Channels;

public sealed class ChannelRepository(SlackCloneDbContext dbContext) : IChannelRepository
{
    public Task<bool> ExistsAsync(Guid workspaceId, Guid channelId, CancellationToken cancellationToken)
    {
        return dbContext.Channels
            .AsNoTracking()
            .AnyAsync(
                c => c.Id == channelId && c.WorkspaceId == workspaceId && !c.IsDeleted,
                cancellationToken);
    }

    public Task<Channel?> GetAsync(Guid workspaceId, Guid channelId, CancellationToken cancellationToken)
    {
        return dbContext.Channels.FirstOrDefaultAsync(
            c => c.Id == channelId && c.WorkspaceId == workspaceId && !c.IsDeleted,
            cancellationToken);
    }

    public Task<bool> NameExistsAsync(Guid workspaceId, string normalizedName, CancellationToken cancellationToken)
    {
        return dbContext.Channels
            .AsNoTracking()
            .AnyAsync(
                c => c.WorkspaceId == workspaceId
                     && c.Name != null
                     && c.Name.ToLower() == normalizedName
                     && !c.IsDeleted,
                cancellationToken);
    }

    public Task AddAsync(Channel channel, CancellationToken cancellationToken)
    {
        dbContext.Channels.Add(channel);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
