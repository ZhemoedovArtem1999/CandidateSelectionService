namespace CandidateSelectionService.Core.Entities
{
    public partial class WorkSchedule
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
