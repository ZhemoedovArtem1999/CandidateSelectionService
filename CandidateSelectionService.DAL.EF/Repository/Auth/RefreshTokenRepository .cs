using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository.Auth;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetValidTokenAsync(int userId, string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    rt.UserId == userId &&
                    rt.ExpiryDate > DateTime.UtcNow &&
                    rt.RevokedDate == null, cancellationToken);
        }

        public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(token, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveOldTokensAsync(int userId, CancellationToken cancellationToken)
        {
            var oldTokens = await _context.RefreshTokens
                .Where(rt =>
                    rt.UserId == userId &&
                    (rt.ExpiryDate <= DateTime.UtcNow || rt.RevokedDate != null))
                .ToListAsync(cancellationToken);

            _context.RefreshTokens.RemoveRange(oldTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeTokenAsync(string token, string replacedByToken = null, CancellationToken cancellationToken = default)
        {
            var refreshToken = await GetByTokenAsync(token, cancellationToken);
            if (refreshToken != null)
            {
                refreshToken.RevokedDate = DateTime.UtcNow;
                refreshToken.ReplacedByToken = replacedByToken;
                await UpdateAsync(refreshToken, cancellationToken);
            }
        }

        public async Task<List<RefreshToken>> GetUserTokensAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
