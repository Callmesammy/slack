using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.EditMessage;

public sealed class EditMessageCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageRepository messageRepository)
    : IRequestHandler<EditMessageCommand, Result<EditMessageResponse>>
{
    public async Task<Result<EditMessageResponse>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<EditMessageResponse>.Failure("VALIDATION_ERROR", "Message content is required.");
        }

        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.EditorUserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<EditMessageResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.EditorUserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<EditMessageResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanPost)
        {
            return Result<EditMessageResponse>.Failure("FORBIDDEN", "No access to edit messages in this channel.");
        }

        var message = await messageRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            cancellationToken);

        if (message is null || message.IsDeleted)
        {
            return Result<EditMessageResponse>.Failure("MESSAGE_NOT_FOUND", "Message not found.");
        }

        if (message.SenderId != request.EditorUserId)
        {
            return Result<EditMessageResponse>.Failure("FORBIDDEN", "Only the sender can edit this message.");
        }

        var now = DateTimeOffset.UtcNow;
        message.Edit(
            request.Content,
            string.IsNullOrWhiteSpace(request.ContentFormat) ? message.ContentFormat : request.ContentFormat,
            now);

        await messageRepository.SaveChangesAsync(cancellationToken);

        return Result<EditMessageResponse>.Success(new EditMessageResponse(
            message.Id,
            message.WorkspaceId,
            message.ChannelId,
            request.EditorUserId,
            now,
            message.Content,
            message.ContentFormat));
    }
}
