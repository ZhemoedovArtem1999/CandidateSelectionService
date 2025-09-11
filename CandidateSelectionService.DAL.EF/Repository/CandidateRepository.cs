using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Filter;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext _context;
        private readonly IDbContextFactory<AppDbContext> contextFactory;

        public CandidateRepository(AppDbContext appDbContext, IDbContextFactory<AppDbContext> contextFactory)
        {
            _context = appDbContext;
            this.contextFactory = contextFactory;
        }

        public async Task AddAsync(Candidate candidate, CancellationToken cancellationToken)
        {
            await _context.Candidates.AddAsync(candidate, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Candidate candidate, CancellationToken cancellationToken)
        {
            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Candidate> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Candidates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Candidate>> GetFilterAsync(CandidateFilter filter, CancellationToken cancellationToken)
        {
            await using var context = contextFactory.CreateDbContext();

            var query = context.Candidates.AsQueryable();

            query = query
                .Include(x => x.DataCandidate)
                .ThenInclude(x => x.SocialNetworks)
                .ThenInclude(x => x.SocialNetworkType)
                .Include(x => x.WorkSchedule);

            query = ApplyFilter(query, filter);

            return await query.ToListAsync(cancellationToken);
        }

        private IQueryable<Candidate> ApplyFilter(IQueryable<Candidate> query, CandidateFilter filter)
        {
            if (filter == null)
            {
                return query;
            }

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                query = query.Where(x => x.DataCandidate.LastName == filter.LastName || x.DataCandidate.SocialNetworks.Where(y => y.LastName == filter.LastName).Any());
            }

            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                query = query.Where(x => x.DataCandidate.FirstName == filter.FirstName || x.DataCandidate.SocialNetworks.Where(y => y.FirstName == filter.FirstName).Any());
            }

            if (!string.IsNullOrEmpty(filter.MiddleName))
            {
                query = query.Where(x => x.DataCandidate.MiddleName == filter.MiddleName);
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(x => x.DataCandidate.Email == filter.Email);
            }

            if (filter.WorkScheduleIds.Any())
            {
                query = query.Where(x => filter.WorkScheduleIds.Any(y => y == x.WorkScheduleId));
            }

            if (filter.IsOnlyMine && filter.CreateUserId.HasValue)
            {
                query = query.Where(x => x.CreatedUserId == filter.CreateUserId);
            }

            if (filter.SortByLastUpdate)
            {
                query = query.OrderByDescending(x => x.LastUpdated);
            }

            if (filter.PageNumber > 0)
            {
                query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            }

            return query;
        }

        public async Task UpdateAsync(Candidate candidate, CancellationToken cancellationToken)
        {
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
