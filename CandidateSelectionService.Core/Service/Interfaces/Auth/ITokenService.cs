using CandidateSelectionService.Core.Entities;
using System.Security.Claims;

namespace CandidateSelectionService.Core.Service.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool IsTokenExpired(string token);
    }
}
