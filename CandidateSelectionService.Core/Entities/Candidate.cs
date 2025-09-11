namespace CandidateSelectionService.Core.Entities
{
    public partial class Candidate
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public virtual DataCandidate DataCandidate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int CreatedUserId { get; set; }
        public int WorkScheduleId { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }

}
