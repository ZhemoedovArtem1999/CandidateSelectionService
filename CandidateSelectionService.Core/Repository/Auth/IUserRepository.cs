using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository.Auth
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken);
        Task CreateAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task<bool> UserExistsAsync(string login, CancellationToken cancellationToken);
    }
}
