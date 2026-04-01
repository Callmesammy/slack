using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SlackClone.Application.Features.Messages.AddReaction;
using SlackClone.Application.Features.Messages.RemoveReaction;
using SlackClone.API.Hubs;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels/{channelId:guid}/messages/{messageId:guid}/reactions")]
public sealed class ReactionsController(ISender sender, IHubContext<ChatHub> hubContext) : ControllerBase
{
    public sealed record ReactionRequest(string Emoji);

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        [FromBody] ReactionRequest request,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new AddReactionCommand(workspaceId, channelId, messageId, userId, request.Emoji),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                "MESSAGE_NOT_FOUND" => NotFound(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        await hubContext.Clients
            .Group(ChatHub.ChannelGroup(channelId))
            .SendAsync(
                ChatEvents.ReactionAdded,
                new
                {
                    messageId = result.Value!.MessageId,
                    workspaceId,
                    channelId,
                    userId,
                    emoji = result.Value!.Emoji,
                    count = result.Value!.Count
                },
                cancellationToken);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Remove(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        [FromBody] ReactionRequest request,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new RemoveReactionCommand(workspaceId, channelId, messageId, userId, request.Emoji),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                "MESSAGE_NOT_FOUND" => NotFound(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        await hubContext.Clients
            .Group(ChatHub.ChannelGroup(channelId))
            .SendAsync(
                ChatEvents.ReactionRemoved,
                new
                {
                    messageId = result.Value!.MessageId,
                    workspaceId,
                    channelId,
                    userId,
                    emoji = result.Value!.Emoji,
                    count = result.Value!.Count
                },
                cancellationToken);

        return Ok(result.Value);
    }
}
