using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Filter;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        private readonly IDbContextFactory<AppDbContext> contextFactory;

        public EmployeeRepository(AppDbContext appDbContext, IDbContextFactory<AppDbContext> contextFactory)
        {
            _context = appDbContext;
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Employee>> GetFilterAsync(EmployeeFilter filter, CancellationToken cancellationToken)
        {
            await using var context = contextFactory.CreateDbContext();

            var query = context.Employees.AsQueryable();

            query = query
                .Include(x => x.DataCandidate)
                .ThenInclude(x => x.SocialNetworks)
                .ThenInclude(x => x.SocialNetworkType)
                .Include(x => x.WorkSchedule);

            query = ApplyFilter(query, filter);

            return await query.ToListAsync(cancellationToken);

        }

        private IQueryable<Employee> ApplyFilter(IQueryable<Employee> query, EmployeeFilter filter)
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

            return query;
        }

        public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            await _context.Employees.AddAsync(employee, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
