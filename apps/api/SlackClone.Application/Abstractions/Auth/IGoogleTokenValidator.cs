namespace SlackClone.Application.Abstractions.Auth;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken);
}

