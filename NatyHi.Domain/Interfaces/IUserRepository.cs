using NatyHi.Domain.Entities;

namespace NatyHi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}
