using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.LeaveChannel;

public sealed class LeaveChannelCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelRepository channelRepository,
    IChannelMemberRepository channelMemberRepository)
    : IRequestHandler<LeaveChannelCommand, Result<LeaveChannelResponse>>
{
    public async Task<Result<LeaveChannelResponse>> Handle(LeaveChannelCommand request, CancellationToken cancellationToken)
    {
        var isWorkspaceMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isWorkspaceMember)
        {
            return Result<LeaveChannelResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var channel = await channelRepository.GetAsync(request.WorkspaceId, request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return Result<LeaveChannelResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        var membership = await channelMemberRepository.GetActiveAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (membership is null)
        {
            return Result<LeaveChannelResponse>.Success(new LeaveChannelResponse(request.ChannelId));
        }

        var now = DateTimeOffset.UtcNow;
        membership.Leave(now);
        channel.DecrementMemberCount(now);

        await channelMemberRepository.SaveChangesAsync(cancellationToken);
        await channelRepository.SaveChangesAsync(cancellationToken);

        return Result<LeaveChannelResponse>.Success(new LeaveChannelResponse(request.ChannelId));
    }
}

