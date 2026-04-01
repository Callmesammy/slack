namespace SlackClone.Application.Abstractions.Channels;

public interface IChannelRepository
{
    Task<bool> ExistsAsync(Guid workspaceId, Guid channelId, CancellationToken cancellationToken);

    Task<SlackClone.Domain.Channels.Channel?> GetAsync(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(Guid workspaceId, string normalizedName, CancellationToken cancellationToken);

    Task AddAsync(SlackClone.Domain.Channels.Channel channel, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
