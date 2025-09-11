using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Filter;

namespace CandidateSelectionService.Core.Repository
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<Candidate>> GetFilterAsync(CandidateFilter filter, CancellationToken cancellationToken);
        Task<Candidate> GetAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Candidate candidate, CancellationToken cancellationToken);
        Task UpdateAsync(Candidate candidate, CancellationToken cancellationToken);
        Task DeleteAsync(Candidate candidate, CancellationToken cancellationToken);
    }
}
