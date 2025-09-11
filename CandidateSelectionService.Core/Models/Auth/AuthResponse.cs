namespace CandidateSelectionService.Core.Models.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
