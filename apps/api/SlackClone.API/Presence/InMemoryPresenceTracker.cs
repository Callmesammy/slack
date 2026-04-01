using System.Collections.Concurrent;

namespace SlackClone.API.Presence;

public sealed class InMemoryPresenceTracker(TimeProvider timeProvider)
{
    private readonly ConcurrentDictionary<(Guid WorkspaceId, Guid UserId), DateTimeOffset> lastSeen = new();

    public bool Touch(Guid workspaceId, Guid userId)
    {
        var key = (workspaceId, userId);
        var now = timeProvider.GetUtcNow();
        var wasOnline = lastSeen.ContainsKey(key);
        lastSeen[key] = now;
        return !wasOnline;
    }

    public IReadOnlyList<(Guid WorkspaceId, Guid UserId)> Expire(TimeSpan ttl)
    {
        var now = timeProvider.GetUtcNow();
        var expired = new List<(Guid WorkspaceId, Guid UserId)>();

        foreach (var kvp in lastSeen)
        {
            if (now - kvp.Value > ttl)
            {
                if (lastSeen.TryRemove(kvp.Key, out _))
                {
                    expired.Add(kvp.Key);
                }
            }
        }

        return expired;
    }
}

