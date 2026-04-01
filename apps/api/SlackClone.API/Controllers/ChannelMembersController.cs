using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Channels.ListChannelMembers;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels/{channelId:guid}/members")]
public sealed class ChannelMembersController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new ListChannelMembersQuery(workspaceId, channelId, userId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }
}

