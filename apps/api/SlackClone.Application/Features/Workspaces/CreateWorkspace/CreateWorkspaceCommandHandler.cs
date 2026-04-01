using MediatR;
using SlackClone.Application.Abstractions.Channels;
using SlackClone.Application.Abstractions.Workspaces;
using SlackClone.Application.Common;
using SlackClone.Domain.Channels;
using SlackClone.Domain.Workspaces;

namespace SlackClone.Application.Features.Workspaces.CreateWorkspace;

public sealed class CreateWorkspaceCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceMemberRepository workspaceMemberRepository,
    IChannelRepository channelRepository,
    IChannelMemberRepository channelMemberRepository)
    : IRequestHandler<CreateWorkspaceCommand, Result<CreateWorkspaceResponse>>
{
    public async Task<Result<CreateWorkspaceResponse>> Handle(
        CreateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<CreateWorkspaceResponse>.Failure("VALIDATION_ERROR", "Workspace name is required.");
        }

        var baseSlug = Slugify(request.Name);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            return Result<CreateWorkspaceResponse>.Failure("VALIDATION_ERROR", "Workspace name is invalid.");
        }

        var slug = baseSlug;
        var suffix = 1;
        while (await workspaceRepository.SlugExistsAsync(slug, cancellationToken))
        {
            suffix++;
            slug = $"{baseSlug}-{suffix}";
            if (suffix > 50)
            {
                return Result<CreateWorkspaceResponse>.Failure("SLUG_UNAVAILABLE", "Could not allocate workspace slug.");
            }
        }

        var now = DateTimeOffset.UtcNow;
        var workspace = Workspace.Create(request.Name, slug, request.OwnerUserId, now, request.Description);
        await workspaceRepository.AddAsync(workspace, cancellationToken);

        var ownerMember = WorkspaceMember.CreateOwner(workspace.Id, request.OwnerUserId, now);
        await workspaceMemberRepository.AddAsync(ownerMember, cancellationToken);

        var general = Channel.CreatePublicChannel(workspace.Id, "general", request.OwnerUserId, now, "Company-wide announcements and work-based matters");
        var random = Channel.CreatePublicChannel(workspace.Id, "random", request.OwnerUserId, now, "Non-work banter and watercooler conversation");

        await channelRepository.AddAsync(general, cancellationToken);
        await channelRepository.AddAsync(random, cancellationToken);

        await channelMemberRepository.AddAsync(
            ChannelMember.Join(workspace.Id, general.Id, request.OwnerUserId, now, role: "owner"),
            cancellationToken);

        await channelMemberRepository.AddAsync(
            ChannelMember.Join(workspace.Id, random.Id, request.OwnerUserId, now, role: "owner"),
            cancellationToken);

        await workspaceRepository.SaveChangesAsync(cancellationToken);
        await workspaceMemberRepository.SaveChangesAsync(cancellationToken);
        await channelRepository.SaveChangesAsync(cancellationToken);
        await channelMemberRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateWorkspaceResponse>.Success(new CreateWorkspaceResponse(workspace.Id, workspace.Slug));
    }

    private static string Slugify(string input)
    {
        var chars = input.Trim().ToLowerInvariant();
        Span<char> buffer = stackalloc char[chars.Length];
        var j = 0;
        var lastWasDash = false;
        foreach (var c in chars)
        {
            var isAlphaNum = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
            if (isAlphaNum)
            {
                buffer[j++] = c;
                lastWasDash = false;
                continue;
            }

            if (!lastWasDash && j > 0)
            {
                buffer[j++] = '-';
                lastWasDash = true;
            }
        }

        var slug = new string(buffer[..j]).Trim('-');
        return slug.Length > 100 ? slug[..100].Trim('-') : slug;
    }
}
