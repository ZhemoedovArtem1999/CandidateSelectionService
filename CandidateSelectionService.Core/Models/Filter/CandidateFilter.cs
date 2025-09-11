using CandidateSelectionService.Core.Models.Common;

namespace CandidateSelectionService.Core.Models.Filter
{
    public class CandidateFilter : PageFilter
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public bool SortByLastUpdate { get; set; } = false;
        public List<int> WorkScheduleIds { get; set; } = new List<int>();
        public bool IsOnlyMine { get; set; } = false;
        public int? CreateUserId { get; private set; }
        public void SetCreateUserId(int value) => CreateUserId = value;
    }
}
