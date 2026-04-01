using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Messages.ListChannelMessages;
using SlackClone.Application.Features.Messages.ListChannelMessagesPage;
using System.IdentityModel.Tokens.Jwt;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels/{channelId:guid}/messages")]
public sealed class ChannelMessagesController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(
        Guid workspaceId,
        Guid channelId,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new ListChannelMessagesQuery(workspaceId, channelId, userId, limit ?? 50),
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

    [Authorize]
    [HttpGet("page")]
    public async Task<IActionResult> Page(
        Guid workspaceId,
        Guid channelId,
        [FromQuery] int? limit,
        [FromQuery] Guid? cursorMessageId,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new ListChannelMessagesPageQuery(workspaceId, channelId, userId, limit ?? 50, cursorMessageId),
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
