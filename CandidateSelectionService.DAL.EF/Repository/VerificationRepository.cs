using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly AppDbContext _context;

        public VerificationRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<int> AddAsync(Verification verification, CancellationToken cancellationToken)
        {
            await _context.Verifications.AddAsync(verification, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return verification.Id;
        }

        public async Task<Verification> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Verifications
                .Include(x => x.VerificationEvents)
                .ThenInclude(x => x.VerificationEventResults)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
