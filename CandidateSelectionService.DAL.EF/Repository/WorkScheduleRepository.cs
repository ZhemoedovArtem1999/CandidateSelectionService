using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace CandidateSelectionService.DAL.EF.Repository
{
    public class WorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly AppDbContext _context;

        public WorkScheduleRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<WorkSchedule> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.WorkSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
