using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using SlackClone.API.Presence;

namespace SlackClone.API.Hubs;

[Authorize]
public sealed class ChatHub(InMemoryPresenceTracker presenceTracker) : Hub
{
    public Task JoinWorkspace(Guid workspaceId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, WorkspaceGroup(workspaceId));
    }

    public Task LeaveWorkspace(Guid workspaceId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, WorkspaceGroup(workspaceId));
    }

    public Task JoinChannel(Guid channelId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, ChannelGroup(channelId));
    }

    public Task LeaveChannel(Guid channelId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChannelGroup(channelId));
    }

    public Task TypingStart(Guid channelId)
    {
        var userId = Context.User?.FindFirstValue("sub");
        return Clients
            .OthersInGroup(ChannelGroup(channelId))
            .SendAsync(ChatEvents.TypingStart, new { channelId, userId });
    }

    public Task TypingStop(Guid channelId)
    {
        var userId = Context.User?.FindFirstValue("sub");
        return Clients
            .OthersInGroup(ChannelGroup(channelId))
            .SendAsync(ChatEvents.TypingStop, new { channelId, userId });
    }

    public Task PresenceHeartbeat(Guid workspaceId)
    {
        var sub = Context.User?.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var userId))
        {
            return Task.CompletedTask;
        }

        var becameOnline = presenceTracker.Touch(workspaceId, userId);
        if (!becameOnline)
        {
            return Task.CompletedTask;
        }

        return Clients
            .Group(WorkspaceGroup(workspaceId))
            .SendAsync(ChatEvents.PresenceOnline, new { workspaceId, userId });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    internal static string ChannelGroup(Guid channelId) => $"channel:{channelId}";
    internal static string WorkspaceGroup(Guid workspaceId) => $"workspace:{workspaceId}";
}
