using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.ListChannelMessages;

public sealed class ListChannelMessagesQueryHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageQueries messageQueries)
    : IRequestHandler<ListChannelMessagesQuery, Result<IReadOnlyList<ChannelMessageDto>>>
{
    public async Task<Result<IReadOnlyList<ChannelMessageDto>>> Handle(
        ListChannelMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<IReadOnlyList<ChannelMessageDto>>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<IReadOnlyList<ChannelMessageDto>>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanRead)
        {
            return Result<IReadOnlyList<ChannelMessageDto>>.Failure("FORBIDDEN", "No access to read this channel.");
        }

        var limit = request.Limit is > 0 and <= 100 ? request.Limit : 50;

        var messages = await messageQueries.ListChannelMessagesAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            limit,
            cancellationToken);

        return Result<IReadOnlyList<ChannelMessageDto>>.Success(messages);
    }
}
