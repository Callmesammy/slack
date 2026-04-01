using SlackClone.Domain.Users;

namespace SlackClone.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> FindByGoogleIdAsync(string googleId, CancellationToken cancellationToken);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

