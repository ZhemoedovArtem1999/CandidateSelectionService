using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface ISocialNetworkRepository
    {
        Task<IEnumerable<SocialNetwork>> GetAsync(int dataCandidateId, IEnumerable<int> ids, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<SocialNetwork> socialNetwork, CancellationToken cancellationToken);
        Task UpdateRangeAsync(IEnumerable<SocialNetwork> socialNetworks, CancellationToken cancellationToken);
        Task DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    }
}
