namespace CandidateSelectionService.Core.Models.Common
{
    public abstract class PageFilter
    {
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 1;
    }
}
