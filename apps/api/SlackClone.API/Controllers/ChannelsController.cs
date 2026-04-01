using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlackClone.Application.Features.Channels.CreateChannel;
using SlackClone.Application.Features.Channels.JoinChannel;
using SlackClone.Application.Features.Channels.LeaveChannel;
using SlackClone.Application.Features.Channels.ListWorkspaceChannels;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels")]
public sealed class ChannelsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(Guid workspaceId, CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(new ListWorkspaceChannelsQuery(workspaceId, userId), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }

    public sealed record CreateChannelRequest(string Name, string? Description, string Type);

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        Guid workspaceId,
        [FromBody] CreateChannelRequest request,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new CreateChannelCommand(
                workspaceId,
                userId,
                request.Name,
                request.Description,
                string.IsNullOrWhiteSpace(request.Type) ? "public" : request.Type),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NAME_TAKEN" => Conflict(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("{channelId:guid}/join")]
    public async Task<IActionResult> Join(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(new JoinChannelCommand(workspaceId, channelId, userId), cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                "CHANNEL_ARCHIVED" => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("{channelId:guid}/leave")]
    public async Task<IActionResult> Leave(
        Guid workspaceId,
        Guid channelId,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(new LeaveChannelCommand(workspaceId, channelId, userId), cancellationToken);
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
