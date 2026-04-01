using Microsoft.EntityFrameworkCore;
using SlackClone.Application.Abstractions.Persistence;
using SlackClone.Domain.Users;
using SlackClone.Infrastructure.Persistence;

namespace SlackClone.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(SlackCloneDbContext dbContext) : IUserRepository
{
    public Task<User?> FindByGoogleIdAsync(string googleId, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

