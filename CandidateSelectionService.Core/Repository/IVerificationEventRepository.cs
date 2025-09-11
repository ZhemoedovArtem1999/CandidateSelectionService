using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface IVerificationEventRepository
    {
        Task<int> AddAsync(VerificationEvent verificationEvent, CancellationToken cancellationToken);
    }
}
