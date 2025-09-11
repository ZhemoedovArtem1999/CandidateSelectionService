using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface IVerificationRepository
    {
        Task<Verification> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> AddAsync(Verification verification, CancellationToken cancellationToken);
    }
}
