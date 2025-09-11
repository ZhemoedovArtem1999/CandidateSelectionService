using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class SocialNetworkRepository : ISocialNetworkRepository
    {
        private readonly AppDbContext _context;

        public SocialNetworkRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IEnumerable<SocialNetwork>> GetAsync(int dataCandidateId, IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            return await _context.SocialNetworks.Where(x => x.DataCandidateId == dataCandidateId && ids.Any(y => y == x.Id)).ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SocialNetwork> socialNetwork, CancellationToken cancellationToken)
        {
            await _context.SocialNetworks.AddRangeAsync(socialNetwork, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<SocialNetwork> socialNetworks, CancellationToken cancellationToken)
        {
            _context.SocialNetworks.UpdateRange(socialNetworks);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var entitiesToDelete = await _context.SocialNetworks.Where(x => ids.Any(y => y == x.Id)).ToListAsync(cancellationToken);

            _context.SocialNetworks.RemoveRange(entitiesToDelete);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
