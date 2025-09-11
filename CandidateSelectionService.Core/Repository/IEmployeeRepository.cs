using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Filter;

namespace CandidateSelectionService.Core.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetFilterAsync(EmployeeFilter filter, CancellationToken cancellationToken);
        Task AddAsync(Employee employee, CancellationToken cancellationToken);
    }
}
