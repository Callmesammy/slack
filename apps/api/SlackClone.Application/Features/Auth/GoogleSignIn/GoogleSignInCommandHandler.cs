using MediatR;
using SlackClone.Application.Abstractions.Auth;
using SlackClone.Application.Abstractions.Persistence;
using SlackClone.Application.Common;
using SlackClone.Domain.Users;

namespace SlackClone.Application.Features.Auth.GoogleSignIn;

public sealed class GoogleSignInCommandHandler(
    IGoogleTokenValidator googleTokenValidator,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<GoogleSignInCommand, Result<GoogleSignInResponse>>
{
    public async Task<Result<GoogleSignInResponse>> Handle(
        GoogleSignInCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            return Result<GoogleSignInResponse>.Failure("INVALID_ID_TOKEN", "ID token is required.");
        }

        GoogleUserInfo googleUser;
        try
        {
            googleUser = await googleTokenValidator.ValidateIdTokenAsync(request.IdToken, cancellationToken);
        }
        catch
        {
            return Result<GoogleSignInResponse>.Failure("INVALID_ID_TOKEN", "Invalid Google ID token.");
        }

        var user = await userRepository.FindByGoogleIdAsync(googleUser.GoogleId, cancellationToken)
                   ?? await userRepository.FindByEmailAsync(googleUser.Email, cancellationToken);

        var now = DateTimeOffset.UtcNow;

        if (user is null)
        {
            user = User.CreateFromGoogle(
                email: googleUser.Email,
                name: googleUser.Name,
                googleId: googleUser.GoogleId,
                avatarUrl: googleUser.AvatarUrl,
                timezone: "UTC",
                now: now);

            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user.UpdateFromGoogle(googleUser.Name, googleUser.AvatarUrl, now);
        }

        await userRepository.SaveChangesAsync(cancellationToken);

        var jwt = jwtTokenGenerator.CreateToken(user.Id, user.Email, user.Name);
        return Result<GoogleSignInResponse>.Success(new GoogleSignInResponse(jwt));
    }
}

