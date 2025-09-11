using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository.Auth
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetValidTokenAsync(int userId, string token, CancellationToken cancellationToken);
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
        Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken);
        Task RemoveOldTokensAsync(int userId, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string replacedByToken = null, CancellationToken cancellationToken = default);
        Task<List<RefreshToken>> GetUserTokensAsync(int userId, CancellationToken cancellationToken);
    }
}
