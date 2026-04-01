using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Auth.GoogleSignIn;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    public sealed record GoogleSignInRequest(string IdToken);

    [HttpPost("google")]
    public async Task<IActionResult> Google([FromBody] GoogleSignInRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GoogleSignInCommand(request.IdToken), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "INVALID_ID_TOKEN" => Unauthorized(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(new { jwt = result.Value!.Jwt });
    }
}

