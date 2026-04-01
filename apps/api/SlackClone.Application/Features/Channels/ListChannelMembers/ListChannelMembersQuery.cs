using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Channels.ListChannelMembers;

public sealed record ListChannelMembersQuery(Guid WorkspaceId, Guid ChannelId, Guid UserId)
    : IRequest<Result<IReadOnlyList<ChannelMemberDto>>>;

public sealed record ChannelMemberDto(
    Guid UserId,
    string Email,
    string? Name,
    string Role,
    DateTimeOffset JoinedAt);

