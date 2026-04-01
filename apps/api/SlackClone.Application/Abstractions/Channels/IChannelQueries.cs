using SlackClone.Application.Features.Channels.ListWorkspaceChannels;

namespace SlackClone.Application.Abstractions.Channels;

public interface IChannelQueries
{
    Task<IReadOnlyList<WorkspaceChannelDto>> ListWorkspaceChannelsAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken);
}
