using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class DataCandidateRepository : IDataCandidateRepository
    {
        private readonly AppDbContext _context;

        public DataCandidateRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<int> AddAsync(DataCandidate data, CancellationToken cancellationToken)
        {
            await _context.DataCandidates.AddAsync(data, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return data.Id;
        }

        public async Task<DataCandidate> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.DataCandidates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(DataCandidate data, CancellationToken cancellationToken)
        {
            _context.DataCandidates.Update(data);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
