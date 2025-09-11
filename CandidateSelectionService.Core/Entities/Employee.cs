namespace CandidateSelectionService.Core.Entities
{
    public partial class Employee
    {
        public int Id { get; set; }
        public int DataId { get; set; }
        public virtual DataCandidate DataCandidate { get; set; }
        public DateTime EmploymentDate { get; set; }
        public int WorkScheduleId { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }
}
