namespace CandidateSelectionService.Core.Entities
{
    public partial class User
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Login { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
