using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SlackClone.Application.Features.Messages.DeleteMessage;
using SlackClone.Application.Features.Messages.EditMessage;
using SlackClone.API.Hubs;

namespace SlackClone.API.Controllers;

[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/channels/{channelId:guid}/messages/{messageId:guid}")]
public sealed class MessageActionsController(ISender sender, IHubContext<ChatHub> hubContext) : ControllerBase
{
    public sealed record EditMessageRequest(string Content, string ContentFormat);

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> Edit(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        [FromBody] EditMessageRequest request,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new EditMessageCommand(
                workspaceId,
                channelId,
                messageId,
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
                "MESSAGE_NOT_FOUND" => NotFound(result.Error),
                "VALIDATION_ERROR" => BadRequest(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        await hubContext.Clients
            .Group(ChatHub.ChannelGroup(channelId))
            .SendAsync(
                ChatEvents.MessageEdited,
                new
                {
                    messageId = result.Value!.MessageId,
                    workspaceId = result.Value!.WorkspaceId,
                    channelId = result.Value!.ChannelId,
                    editorUserId = result.Value!.EditorUserId,
                    editedAt = result.Value!.EditedAt,
                    content = result.Value!.Content,
                    contentFormat = result.Value!.ContentFormat
                },
                cancellationToken);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        CancellationToken cancellationToken)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(sub, out var userId))
        {
            return Unauthorized(new { code = "UNAUTHORISED", message = "Invalid user." });
        }

        var result = await sender.Send(
            new DeleteMessageCommand(workspaceId, channelId, messageId, userId),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error?.Code switch
            {
                "FORBIDDEN" => Forbid(),
                "CHANNEL_NOT_FOUND" => NotFound(result.Error),
                "MESSAGE_NOT_FOUND" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        await hubContext.Clients
            .Group(ChatHub.ChannelGroup(channelId))
            .SendAsync(
                ChatEvents.MessageDeleted,
                new
                {
                    messageId = result.Value!.MessageId,
                    workspaceId = result.Value!.WorkspaceId,
                    channelId = result.Value!.ChannelId,
                    deleterUserId = result.Value!.DeleterUserId,
                    deletedAt = result.Value!.DeletedAt
                },
                cancellationToken);

        return Ok(result.Value);
    }
}
