using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Domain.Messages;

namespace SlackClone.Application.Features.Messages.AddReaction;

public sealed class AddReactionCommandHandler(
    IWorkspaceMembershipService workspaceMembershipService,
    IChannelAccessService channelAccessService,
    IMessageRepository messageRepository,
    IMessageReactionRepository reactionRepository,
    IMessageQueries messageQueries)
    : IRequestHandler<AddReactionCommand, Result<AddReactionResponse>>
{
    public async Task<Result<AddReactionResponse>> Handle(AddReactionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Emoji))
        {
            return Result<AddReactionResponse>.Failure("VALIDATION_ERROR", "Emoji is required.");
        }

        var isMember = await workspaceMembershipService.IsMemberAsync(
            request.WorkspaceId,
            request.UserId,
            cancellationToken);

        if (!isMember)
        {
            return Result<AddReactionResponse>.Failure("FORBIDDEN", "Not a workspace member.");
        }

        var access = await channelAccessService.CheckAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.UserId,
            cancellationToken);

        if (!access.Exists)
        {
            return Result<AddReactionResponse>.Failure("CHANNEL_NOT_FOUND", "Channel not found.");
        }

        if (!access.CanRead)
        {
            return Result<AddReactionResponse>.Failure("FORBIDDEN", "No access to react in this channel.");
        }

        var message = await messageRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            cancellationToken);

        if (message is null || message.IsDeleted)
        {
            return Result<AddReactionResponse>.Failure("MESSAGE_NOT_FOUND", "Message not found.");
        }

        var existing = await reactionRepository.GetAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            request.UserId,
            request.Emoji,
            cancellationToken);

        if (existing is not null)
        {
            var count = await messageQueries.GetReactionCountAsync(
                request.WorkspaceId,
                request.ChannelId,
                request.MessageId,
                existing.Emoji,
                cancellationToken);

            return Result<AddReactionResponse>.Success(new AddReactionResponse(request.MessageId, existing.Emoji, count));
        }

        var now = DateTimeOffset.UtcNow;
        var reaction = MessageReaction.Create(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            request.UserId,
            request.Emoji,
            now);

        await reactionRepository.AddAsync(reaction, cancellationToken);
        await reactionRepository.SaveChangesAsync(cancellationToken);

        var newCount = await messageQueries.GetReactionCountAsync(
            request.WorkspaceId,
            request.ChannelId,
            request.MessageId,
            reaction.Emoji,
            cancellationToken);

        return Result<AddReactionResponse>.Success(new AddReactionResponse(request.MessageId, reaction.Emoji, newCount));
    }
}
