namespace CandidateSelectionService.Core.Entities
{
    public class VerificationEvent
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int VerificationId { get; set; }
        public virtual Verification Verification { get; set; }
        public virtual ICollection<VerificationEventResult> VerificationEventResults { get; set; }
    }
}
