namespace CandidateSelectionService.Core.Entities
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedDate { get; set; }
        public string? ReplacedByToken { get; set; }
        public virtual User User { get; set; }
    }
}
