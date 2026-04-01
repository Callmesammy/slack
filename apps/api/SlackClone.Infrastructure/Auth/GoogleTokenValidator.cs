using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using SlackClone.Application.Abstractions.Auth;

namespace SlackClone.Infrastructure.Auth;

public sealed class GoogleTokenValidator(IConfiguration configuration) : IGoogleTokenValidator
{
    public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken)
    {
        var clientId = configuration["Google:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new InvalidOperationException("Missing Google:ClientId configuration.");
        }

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId],
            }).ConfigureAwait(false);

        return new GoogleUserInfo(
            GoogleId: payload.Subject,
            Email: payload.Email,
            Name: payload.Name ?? payload.Email,
            AvatarUrl: payload.Picture);
    }
}

