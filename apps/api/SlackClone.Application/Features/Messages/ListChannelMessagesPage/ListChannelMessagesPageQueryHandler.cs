using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.ListChannelMessagesPage;

public sealed class ListChannelMessagesPageQueryHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageQueries messageQueries)
    : IRequestHandler<ListChannelMessagesPageQuery, Result<ChannelMessagePageDto>>
{
    public async Task<Result<ChannelMessagePageDto>> Handle(
        ListChannelMessagesPageQuery request,
        CancellationToken cancellationToken)
    {
        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<ChannelMessagePageDto>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<ChannelMessagePageDto>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanRead)
        {
            return Result<ChannelMessagePageDto>.Failure("FORBIDDEN", "No access to read this channel.");
        }

        var page = await messageQueries.ListChannelMessagesPageAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.Limit,
            request.CursorMessageId,
            cancellationToken);

        return Result<ChannelMessagePageDto>.Success(page);
    }
}

