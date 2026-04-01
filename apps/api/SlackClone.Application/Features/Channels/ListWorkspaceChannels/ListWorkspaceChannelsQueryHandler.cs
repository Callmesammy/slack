using MediatR;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.ListWorkspaceChannels;

public sealed class ListWorkspaceChannelsQueryHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelQueries channelQueries)
    : IRequestHandler<ListWorkspaceChannelsQuery, Result<IReadOnlyList<WorkspaceChannelDto>>>
{
    public async Task<Result<IReadOnlyList<WorkspaceChannelDto>>> Handle(
        ListWorkspaceChannelsQuery request,
        CancellationToken cancellationToken)
    {
        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<IReadOnlyList<WorkspaceChannelDto>>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var channels = await channelQueries.ListWorkspaceChannelsAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        return Result<IReadOnlyList<WorkspaceChannelDto>>.Success(channels);
    }
}
