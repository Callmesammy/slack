using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Messages.RemoveReaction;

public sealed class RemoveReactionCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageRepository messageRepository,
    IMessageReactionRepository reactionRepository,
    IMessageQueries messageQueries)
    : IRequestHandler<RemoveReactionCommand, Result<RemoveReactionResponse>>
{
    public async Task<Result<RemoveReactionResponse>> Handle(RemoveReactionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Emoji))
        {
            return Result<RemoveReactionResponse>.Failure("VALIDATION_ERROR", "Emoji is required.");
        }

        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<RemoveReactionResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<RemoveReactionResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanRead)
        {
            return Result<RemoveReactionResponse>.Failure("FORBIDDEN", "No access to react in this channel.");
        }

        var message = await messageRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            cancellationToken);

        if (message is null || message.IsDeleted)
        {
            return Result<RemoveReactionResponse>.Failure("MESSAGE_NOT_FOUND", "Message not found.");
        }

        var existing = await reactionRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            request.UserId,
            request.Emoji,
            cancellationToken);

        if (existing is null)
        {
            var count = await messageQueries.GetReactionCountAsync(
                request.WorkspaceId,
                request.ChannelId,
                request.MessageId,
                request.Emoji,
                cancellationToken);

            return Result<RemoveReactionResponse>.Success(new RemoveReactionResponse(request.MessageId, request.Emoji.Trim(), count));
        }

        await reactionRepository.RemoveAsync(existing, cancellationToken);
        await reactionRepository.SaveChangesAsync(cancellationToken);

        var newCount = await messageQueries.GetReactionCountAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            existing.Emoji,
            cancellationToken);

        return Result<RemoveReactionResponse>.Success(new RemoveReactionResponse(request.MessageId, existing.Emoji, newCount));
    }
}
