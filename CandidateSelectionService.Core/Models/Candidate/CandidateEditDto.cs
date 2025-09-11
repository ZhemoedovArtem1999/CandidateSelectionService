namespace CandidateSelectionService.Core.Models.Candidate
{
    public record CandidateEditDto(
        string LastName,
        string FirstName,
        string MiddleName,
        string Email,
        string Phone,
        string Country,
        DateOnly DateBirth,
        int WorkScheduleId,
        List<SocialNetworkModel>? SocialNetworks);
}
