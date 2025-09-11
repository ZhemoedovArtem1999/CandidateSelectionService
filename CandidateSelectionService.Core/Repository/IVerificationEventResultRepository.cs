using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface IVerificationEventResultRepository
    {
        Task AddRangeAsync(IEnumerable<VerificationEventResult> verificationEventResults, CancellationToken cancellationToken);
    }
}
