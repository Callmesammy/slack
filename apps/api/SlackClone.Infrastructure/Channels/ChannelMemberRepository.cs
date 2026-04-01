using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Domain.Channels;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Channels;

public sealed class ChannelMemberRepository(SlackCloneDbContext dbContext) : IChannelMemberRepository
{
    public Task<ChannelMember?> GetActiveAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.ChannelMembers.FirstOrDefaultAsync(
            m => m.WorkspaceId == workspaceId
                 && m.ChannelId == channelId
                 && m.UserId == userId
                 && m.LeftAt == null,
            cancellationToken);
    }

    public Task AddAsync(ChannelMember member, CancellationToken cancellationToken)
    {
        dbContext.ChannelMembers.Add(member);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

