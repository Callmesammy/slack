using SlackClone.Application.Features.Channels.ListChannelMembers;

namespace SlackClone.Application.Abstractions.Channels;

public interface IChannelMemberQueries
{
    Task<IReadOnlyList<ChannelMemberDto>> ListChannelMembersAsync(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken);
}

