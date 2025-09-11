namespace CandidateSelectionService.Core.Models.Auth
{
    public class RegisterRequest
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
