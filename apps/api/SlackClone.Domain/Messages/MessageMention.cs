using SlackClone.Domain.Users;

namespace SlackClone.Domain.Messages;

public sealed class MessageMention
{
    private MessageMention() { }

    public Guid Id { get; private set; }
    public Guid MessageId { get; private set; }
    public Message? Message { get; private set; }

    public Guid WorkspaceId { get; private set; }
    public Guid ChannelId { get; private set; }

    public Guid? MentionedUserId { get; private set; }
    public User? MentionedUser { get; private set; }

    public string MentionType { get; private set; } = "user";
    public DateTimeOffset CreatedAt { get; private set; }
}

