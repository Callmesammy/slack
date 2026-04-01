using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackClone.Application.Abstractions.Auth;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Messages;
using SlackClone.Application.Abstractions.Persistence;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Infrastructure.Auth;
using SlackClone.Infrastructure.Channels;
using SlackClone.Infrastructure.Messages;
using SlackClone.Infrastructure.Persistence;
using SlackClone.Infrastructure.Persistence.Repositories;
using SlackClone.Infrastructure.Workspaces;

namespace SlackClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<SlackCloneDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChannelQueries, ChannelQueries>();
        services.AddScoped<IChannelAccessService, ChannelAccessService>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IChannelMemberQueries, ChannelMemberQueries>();
        services.AddScoped<IChannelMemberRepository, ChannelMemberRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMessageQueries, MessageQueries>();
        services.AddScoped<IMessageReactionRepository, MessageReactionRepository>();
        services.AddScoped<IWorkspaceMembershipService, WorkspaceMembershipService>();
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<IWorkspaceMemberRepository, WorkspaceMemberRepository>();
        services.AddScoped<IWorkspaceQueries, WorkspaceQueries>();
        services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
