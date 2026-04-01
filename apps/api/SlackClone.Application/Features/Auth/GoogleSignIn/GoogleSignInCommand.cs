using MediatR;
using SlackClone.Application.Common;

namespace SlackClone.Application.Features.Auth.GoogleSignIn;

public sealed record GoogleSignInCommand(string IdToken) : IRequest<Result<GoogleSignInResponse>>;

public sealed record GoogleSignInResponse(string Jwt);

