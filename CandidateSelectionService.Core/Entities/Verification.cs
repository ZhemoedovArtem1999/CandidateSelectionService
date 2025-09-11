namespace CandidateSelectionService.Core.Entities
{
    public class Verification
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string SearchData { get; set; }
        public virtual ICollection<VerificationEvent> VerificationEvents { get; set; }
    }
}
