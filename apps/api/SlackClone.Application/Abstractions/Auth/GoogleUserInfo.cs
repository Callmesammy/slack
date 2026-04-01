namespace SlackClone.Application.Abstractions.Auth;

public sealed record GoogleUserInfo(
    string GoogleId,
    string Email,
    string Name,
    string? AvatarUrl);

