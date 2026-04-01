using Microsoft.EntityFrameworkCore;
using SlackClone.Domain.Channels;
using SlackClone.Domain.Users;
using SlackClone.Domain.Messages;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Infrastructure.Persistence;

public sealed class SlackCloneDbContext(DbContextOptions<SlackCloneDbContext> options) : DbContext(options)
{
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<ChannelMember> ChannelMembers => Set<ChannelMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageMention> MessageMentions => Set<MessageMention>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SlackCloneDbContext).Assembly);
    }
}
