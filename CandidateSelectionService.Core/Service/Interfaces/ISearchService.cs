using CandidateSelectionService.Core.Models.Search;

namespace CandidateSelectionService.Core.Service.Interfaces
{
    public interface ISearchService
    {
        Task<SendData> SearchEntitiesAsync(SearchDataDto search, int userId, CancellationToken cancellationToken);
    }
}
