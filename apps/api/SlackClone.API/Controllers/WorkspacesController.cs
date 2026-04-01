using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Workspaces.CreateWorkspace;
using SlackClone.Application.Features.Workspaces.GetUserWorkspaces;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces")]
public sealed class WorkspacesController(ISender sender) : ControllerBase
{
    public sealed record CreateWorkspaceRequest(string Name, string? Description);

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(new GetUserWorkspacesQuery(userId), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkspaceRequest request,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(new CreateWorkspaceCommand(userId, request.Name, request.Description), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "VALIDATION_ERROR" => BadRequest(result.Error),
                "SLUG_UNAVAILABLE" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }
}
