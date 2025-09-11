using CandidateSelectionService.Core.Models.Search;

namespace CandidateSelectionService.Core.Service.Interfaces
{
    public interface ISendDataService
    {
        Task SendMessageAsync(SendData data, CancellationToken cancellationToken);
    }
}
