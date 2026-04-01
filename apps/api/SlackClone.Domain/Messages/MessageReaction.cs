using SlackClone.Domain.Users;

namespace SlackClone.Domain.Messages;

public sealed class MessageReaction
{
    private MessageReaction() { }

    public Guid Id { get; private set; }

    public Guid WorkspaceId { get; private set; }

    public Guid ChannelId { get; private set; }

    public Guid MessageId { get; private set; }
    public Message? Message { get; private set; }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public string Emoji { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static MessageReaction Create(
        Guid workspaceId,
        Guid channelId,
        Guid messageId,
        Guid userId,
        string emoji,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(emoji)) throw new ArgumentException("Emoji is required.", nameof(emoji));

        return new MessageReaction
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            ChannelId = channelId,
            MessageId = messageId,
            UserId = userId,
            Emoji = emoji.Trim(),
            CreatedAt = now
        };
    }
}

