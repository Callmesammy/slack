using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SlackClone.Application.Features.Messages.SendMessage;
using SlackClone.API.Hubs;
using System.IdentityModel.Tokens.Jwt;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels/{channelId:guid}/messages/send")]
public sealed class MessagesController(ISender sender, IHubContext<ChatHub> hubContext) : ControllerBase
{
    public sealed record SendMessageRequest(string Content, string ContentFormat);

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Send(
        Guid workspaceId,
        Guid channelId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
        var name = User.FindFirst("name")?.Value;

        var result = await sender.Send(
            new SendMessageCommand(
                workspaceId,
                channelId,
                userId,
                request.Content,
                string.IsNullOrWhiteSpace(request.ContentFormat) ? "tiptap" : request.ContentFormat),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        await hubContext.Clients
            .Group(ChatHub.ChannelGroup(channelId))
            .SendAsync(
                ChatEvents.MessageNew,
                new
                {
                    messageId = result.Value!.MessageId,
                    workspaceId = result.Value!.WorkspaceId,
                    channelId = result.Value!.ChannelId,
                    senderId = result.Value!.SenderId,
                    createdAt = result.Value!.CreatedAt,
                    content = result.Value!.Content,
                    contentFormat = result.Value!.ContentFormat,
                    senderEmail = email,
                    senderName = name
                },
                cancellationToken);

        return Ok(result.Value);
    }
}
