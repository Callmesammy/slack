using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.ListChannelMembers;

public sealed class ListChannelMembersQueryHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IChannelMemberQueries channelMemberQueries)
    : IRequestHandler<ListChannelMembersQuery, Result<IReadOnlyList<ChannelMemberDto>>>
{
    public async Task<Result<IReadOnlyList<ChannelMemberDto>>> Handle(
        ListChannelMembersQuery request,
        CancellationToken cancellationToken)
    {
        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<IReadOnlyList<ChannelMemberDto>>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<IReadOnlyList<ChannelMemberDto>>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanRead)
        {
            return Result<IReadOnlyList<ChannelMemberDto>>.Failure("FORBIDDEN", "No access to read this channel.");
        }

        var members = await channelMemberQueries.ListChannelMembersAsync(
            request.WorkspaceId,
            request.ChannelId,
            cancellationToken);

        return Result<IReadOnlyList<ChannelMemberDto>>.Success(members);
    }
}

