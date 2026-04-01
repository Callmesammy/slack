using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.ListWorkspaceChannels;

public sealed record ListWorkspaceChannelsQuery(Guid WorkspaceId, Guid UserId)
    : IRequest<Result<IReadOnlyList<WorkspaceChannelDto>>>;

public sealed record WorkspaceChannelDto(
    Guid Id,
    string? Name,
    string Type,
    bool IsArchived,
    int MemberCount,
    DateTimeOffset? LastMessageAt);
