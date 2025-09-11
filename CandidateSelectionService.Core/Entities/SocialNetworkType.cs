namespace CandidateSelectionService.Core.Entities
{
    public partial class SocialNetworkType
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<SocialNetwork> SocialNetworks { get; set; }
    }
}
