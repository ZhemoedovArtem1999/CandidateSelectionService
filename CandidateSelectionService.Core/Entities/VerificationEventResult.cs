namespace CandidateSelectionService.Core.Entities
{
    public class VerificationEventResult
    {
        public int Id { get; set; }
        public int VerificationEventId { get; set; }
        public int EntityId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateOnly DateBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string WorkSchedule { get; set; }
        public virtual VerificationEvent VerificationEvent { get; set; }
    }
}
