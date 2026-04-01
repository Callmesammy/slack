using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Domain.Messages;

namespace SlackClone.Application.Features.Messages.SendMessage;

public sealed class SendMessageCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageRepository messageRepository)
    : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    public async Task<Result<SendMessageResponse>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<SendMessageResponse>.Failure("VALIDATION_ERROR", "Message content is required.");
        }

        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.SenderId,
            cancellationToken);

        if (!isMember)
        {
            return Result<SendMessageResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.SenderId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<SendMessageResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanPost)
        {
            return Result<SendMessageResponse>.Failure("FORBIDDEN", "No access to post in this channel.");
        }

        var now = DateTimeOffset.UtcNow;

        var message = Message.Create(
            request.WorkspaceId,
            request.ChannelId,
            request.SenderId,
            request.Content,
            request.ContentFormat,
            now);

        await messageRepository.AddAsync(message, cancellationToken);
        await messageRepository.SaveChangesAsync(cancellationToken);

        return Result<SendMessageResponse>.Success(new SendMessageResponse(
            message.Id,
            message.WorkspaceId,
            message.ChannelId,
            message.SenderId,
            message.CreatedAt,
            message.Content,
            message.ContentFormat));
    }
}
