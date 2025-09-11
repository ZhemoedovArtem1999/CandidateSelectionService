using CandidateSelectionService.Core.Entities;

namespace CandidateSelectionService.Core.Repository
{
    public interface IWorkScheduleRepository
    {
        Task<WorkSchedule> GetAsync(int id, CancellationToken cancellationToken);
    }
}
