namespace CandidateSelectionService.Core.Models.Search
{
    public class SendData
    {
        public string UserName { get; set; }
        public string Date { get; set; }
        public string SearchData { get; set; }
        public IEnumerable<EntityItem>? Candidates { get; set; }
        public IEnumerable<EntityItem>? Employess { get; set; }
    }

    public class EntityItem
    {
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string DateBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string WorkSchedule { get; set; }
    }
}
