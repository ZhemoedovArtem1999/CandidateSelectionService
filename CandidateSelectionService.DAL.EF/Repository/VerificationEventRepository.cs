using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class VerificationEventRepository : IVerificationEventRepository
    {
        private readonly AppDbContext _context;

        public VerificationEventRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<int> AddAsync(VerificationEvent verificationEvent, CancellationToken cancellationToken)
        {
            await _context.VerificationEvents.AddAsync(verificationEvent, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return verificationEvent.Id;
        }
    }
}
