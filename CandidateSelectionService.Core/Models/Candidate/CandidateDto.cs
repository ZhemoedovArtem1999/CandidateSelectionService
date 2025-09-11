namespace CandidateSelectionService.Core.Models.Candidate
{
    public record CandidateDto(
        int id,
         string LastName,
        string FirstName,
        string MiddleName,
        string Email,
        string Phone,
        string Country,
        DateOnly DateBirth,
        string WorkSchedule,
        List<SocialNetworkView>? SocialNetworks
        );
}
