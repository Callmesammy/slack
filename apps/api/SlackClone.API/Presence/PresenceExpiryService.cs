using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SlackClone.API.Hubs;

namespace SlackClone.API.Presence;

public sealed class PresenceExpiryService(
    InMemoryPresenceTracker tracker,
    IHubContext<ChatHub> hubContext,
    TimeProvider timeProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ttl = TimeSpan.FromSeconds(60);
        var interval = TimeSpan.FromSeconds(10);

        while (!stoppingToken.IsCancellationRequested)
        {
            var expired = tracker.Expire(ttl);
            foreach (var (workspaceId, userId) in expired)
            {
                await hubContext.Clients
                    .Group(ChatHub.WorkspaceGroup(workspaceId))
                    .SendAsync(ChatEvents.PresenceOffline, new { workspaceId, userId }, stoppingToken);
            }

            await Task.Delay(interval, timeProvider, stoppingToken);
        }
    }
}

