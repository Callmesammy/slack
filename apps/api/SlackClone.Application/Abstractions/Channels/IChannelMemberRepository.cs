using SlackClone.Domain.Channels;

namespace SlackClone.Application.Abstractions.Channels;

public interface IChannelMemberRepository
{
    Task<ChannelMember?> GetActiveAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(ChannelMember member, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

