using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface IDataCandidateRepository
    {
        Task<DataCandidate> GetAsync(int id, CancellationToken cancellationToken);
        Task<int> AddAsync(DataCandidate data, CancellationToken cancellationToken);
        Task UpdateAsync(DataCandidate data, CancellationToken cancellationToken);
    }
}
