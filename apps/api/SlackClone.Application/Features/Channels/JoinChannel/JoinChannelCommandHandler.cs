using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Domain.Channels;

namespace SlackClone.Application.Features.Channels.JoinChannel;

public sealed class JoinChannelCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelRepository channelRepository,
    IChannelMemberRepository channelMemberRepository)
    : IRequestHandler<JoinChannelCommand, Result<JoinChannelResponse>>
{
    public async Task<Result<JoinChannelResponse>> Handle(JoinChannelCommand request, CancellationToken cancellationToken)
    {
        var isWorkspaceMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isWorkspaceMember)
        {
            return Result<JoinChannelResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var channel = await channelRepository.GetAsync(request.WorkspaceId, request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return Result<JoinChannelResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (channel.IsArchived)
        {
            return Result<JoinChannelResponse>.Failure("CHANNEL_ARCHIVED", "Channel is archived.");
        }

        if (channel.Type != ChannelType.Public)
        {
            return Result<JoinChannelResponse>.Failure("FORBIDDEN", "Cannot join a private channel without an invite.");
        }

        var existing = await channelMemberRepository.GetActiveAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (existing is not null)
        {
            return Result<JoinChannelResponse>.Success(new JoinChannelResponse(request.ChannelId));
        }

        var now = DateTimeOffset.UtcNow;
        var member = ChannelMember.Join(request.WorkspaceId, request.ChannelId, request.UserId, now);
        await channelMemberRepository.AddAsync(member, cancellationToken);
        channel.IncrementMemberCount(now);

        await channelMemberRepository.SaveChangesAsync(cancellationToken);
        await channelRepository.SaveChangesAsync(cancellationToken);

        return Result<JoinChannelResponse>.Success(new JoinChannelResponse(request.ChannelId));
    }
}

