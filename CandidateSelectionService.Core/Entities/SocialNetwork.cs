namespace CandidateSelectionService.Core.Entities
{
    public partial class SocialNetwork
    {
        public int Id { get; set; }
        public int DataCandidateId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int TypeId { get; set; }
        public DateTime DateAdded { get; set; }
        public virtual SocialNetworkType SocialNetworkType { get; set; }
        public virtual DataCandidate DataCandidate { get; set; }
    }
}
