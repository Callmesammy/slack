namespace SlackClone.Application.Abstractions.Auth;

public interface IJwtTokenGenerator
{
    string CreateToken(Guid userId, string email, string name);
}

