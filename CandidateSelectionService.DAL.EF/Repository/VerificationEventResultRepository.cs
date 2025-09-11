using CandidateSelectionService.Core.Repository;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class VerificationEventResultRepository : IVerificationEventResultRepository
    {
        private readonly AppDbContext _context;

        public VerificationEventResultRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task AddRangeAsync(IEnumerable<Core.Entities.VerificationEventResult> verificationEventResults, CancellationToken cancellationToken)
        {
            await _context.VerificationEventResults.AddRangeAsync(verificationEventResults, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
