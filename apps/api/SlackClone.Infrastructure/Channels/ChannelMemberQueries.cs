using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Features.Channels.ListChannelMembers;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Channels;

public sealed class ChannelMemberQueries(SlackCloneDbContext dbContext) : IChannelMemberQueries
{
    public async Task<IReadOnlyList<ChannelMemberDto>> ListChannelMembersAsync(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken)
    {
        return await (
                from member in dbContext.ChannelMembers.AsNoTracking()
                join user in dbContext.Users.AsNoTracking() on member.UserId equals user.Id
                where member.WorkspaceId == workspaceId
                      && member.ChannelId == channelId
                      && member.LeftAt == null
                orderby member.JoinedAt
                select new ChannelMemberDto(
                    user.Id,
                    user.Email,
                    user.Name,
                    member.Role,
                    member.JoinedAt))
            .ToListAsync(cancellationToken);
    }
}

