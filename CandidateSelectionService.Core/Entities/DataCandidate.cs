namespace CandidateSelectionService.Core.Entities
{
    public partial class DataCandidate
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public DateOnly DateBirth { get; set; }
        public virtual ICollection<SocialNetwork> SocialNetworks { get; set; }
        public virtual Candidate? Candidate { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
