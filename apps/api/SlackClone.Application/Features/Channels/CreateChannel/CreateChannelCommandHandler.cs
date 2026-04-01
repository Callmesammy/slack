using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Domain.Channels;

namespace SlackClone.Application.Features.Channels.CreateChannel;

public sealed class CreateChannelCommandHandler(
    IChannelRepository channelRepository,
    IWorkspaceMembershipService workspaceMembershipService)
    : IRequestHandler<CreateChannelCommand, Result<CreateChannelResponse>>
{
    public async Task<Result<CreateChannelResponse>> Handle(
        CreateChannelCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<CreateChannelResponse>.Failure("VALIDATION_ERROR", "Channel name is required.");
        }

        if (!await workspaceMembershipService.IsMemberAsync(
                request.WorkspaceId,
                request.CreatorUserId,
                cancellationToken))
        {
            return Result<CreateChannelResponse>.Failure("FORBIDDEN", "You are not a member of this workspace.");
        }

        var normalizedName = request.Name.Trim().ToLowerInvariant();

        var exists = await channelRepository.NameExistsAsync(
            request.WorkspaceId,
            normalizedName,
            cancellationToken);

        if (exists)
        {
            return Result<CreateChannelResponse>.Failure("CHANNEL_NAME_TAKEN", "Channel name is already in use.");
        }

        var now = DateTimeOffset.UtcNow;
        var channel = Channel.CreatePublicChannel(
            request.WorkspaceId,
            request.Name,
            request.CreatorUserId,
            now,
            request.Description);

        await channelRepository.AddAsync(channel, cancellationToken);
        await channelRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateChannelResponse>.Success(new CreateChannelResponse(channel.Id));
    }
}
