using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.DeleteMessage;

public sealed class DeleteMessageCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageRepository messageRepository)
    : IRequestHandler<DeleteMessageCommand, Result<DeleteMessageResponse>>
{
    public async Task<Result<DeleteMessageResponse>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.DeleterUserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<DeleteMessageResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.DeleterUserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<DeleteMessageResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanPost)
        {
            return Result<DeleteMessageResponse>.Failure("FORBIDDEN", "No access to delete messages in this channel.");
        }

        var message = await messageRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            cancellationToken);

        if (message is null || message.IsDeleted)
        {
            return Result<DeleteMessageResponse>.Failure("MESSAGE_NOT_FOUND", "Message not found.");
        }

        if (message.SenderId != request.DeleterUserId)
        {
            return Result<DeleteMessageResponse>.Failure("FORBIDDEN", "Only the sender can delete this message.");
        }

        var now = DateTimeOffset.UtcNow;
        message.SoftDelete(request.DeleterUserId, now);
        await messageRepository.SaveChangesAsync(cancellationToken);

        return Result<DeleteMessageResponse>.Success(new DeleteMessageResponse(
            message.Id,
            message.WorkspaceId,
            message.ChannelId,
            request.DeleterUserId,
            now));
    }
}
