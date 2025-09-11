using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository.Auth;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
        }

        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UserExistsAsync(string login, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Login == login, cancellationToken);
        }
    }
}
