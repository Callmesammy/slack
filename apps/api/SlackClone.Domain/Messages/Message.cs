using SlackClone.Domain.Channels;
using SlackClone.Domain.Users;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Domain.Messages;

public sealed class Message
{
    private Message() { }

    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Workspace? Workspace { get; private set; }

    public Guid ChannelId { get; private set; }
    public Channel? Channel { get; private set; }

    public Guid SenderId { get; private set; }
    public User? Sender { get; private set; }

    public string Content { get; private set; } = string.Empty;
    public string ContentFormat { get; private set; } = "tiptap";
    public MessageType Type { get; private set; } = MessageType.Text;

    public Guid? ThreadParentId { get; private set; }
    public Message? ThreadParent { get; private set; }

    public int ReplyCount { get; private set; }
    public DateTimeOffset? LatestReplyAt { get; private set; }
    public Guid[]? LatestReplySenderIds { get; private set; }

    public bool IsPinned { get; private set; }
    public DateTimeOffset? PinnedAt { get; private set; }
    public Guid? PinnedBy { get; private set; }

    public bool IsEdited { get; private set; }
    public DateTimeOffset? EditedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public string? Metadata { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void Edit(string content, string contentFormat, DateTimeOffset now)
    {
        if (IsDeleted) throw new InvalidOperationException("Cannot edit a deleted message.");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required.", nameof(content));
        if (string.IsNullOrWhiteSpace(contentFormat)) throw new ArgumentException("ContentFormat is required.", nameof(contentFormat));

        Content = content;
        ContentFormat = contentFormat;
        IsEdited = true;
        EditedAt = now;
        UpdatedAt = now;
    }

    public void SoftDelete(Guid deletedBy, DateTimeOffset now)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = now;
        DeletedBy = deletedBy;
        UpdatedAt = now;
    }

    public static Message Create(
        Guid workspaceId,
        Guid channelId,
        Guid senderId,
        string content,
        string contentFormat,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required.", nameof(content));
        if (string.IsNullOrWhiteSpace(contentFormat)) throw new ArgumentException("ContentFormat is required.", nameof(contentFormat));

        return new Message
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            ChannelId = channelId,
            SenderId = senderId,
            Content = content,
            ContentFormat = contentFormat,
            Type = MessageType.Text,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
