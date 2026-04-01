namespace SlackClone.API.Hubs;

public static class ChatEvents
{
    public const string MessageNew = "message:new";
    public const string MessageEdited = "message:edited";
    public const string MessageDeleted = "message:deleted";
    public const string ReactionAdded = "reaction:added";
    public const string ReactionRemoved = "reaction:removed";
    public const string TypingStart = "typing:start";
    public const string TypingStop = "typing:stop";
    public const string PresenceOnline = "presence:online";
    public const string PresenceOffline = "presence:offline";
}
