namespace CandidateSelectionService.Core.Models.Candidate
{
    public record SocialNetworkModel(
        int? Id,
        string LastName,
        string FirstName,
        int TypeId,
        bool IsDeleted = false
        );
}
