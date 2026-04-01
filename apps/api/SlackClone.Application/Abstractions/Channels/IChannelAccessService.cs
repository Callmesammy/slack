namespace SlackClone.Application.Abstractions.Channels;

public sealed record ChannelAccessCheck(
    bool Exists,
    bool CanRead,
    bool CanPost);

public interface IChannelAccessService
{
    Task<ChannelAccessCheck> CheckAsync(
        Guid workspaceId,
        Guid channelId,
        Guid userId,
        CancellationToken cancellationToken);
}

