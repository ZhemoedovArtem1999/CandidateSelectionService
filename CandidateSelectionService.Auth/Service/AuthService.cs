using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Auth;
using CandidateSelectionService.Core.Repository.Auth;
using CandidateSelectionService.Core.Service.Interfaces.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CandidateSelectionService.Auth.Service
{
    public class AuthService : IAuthService
    {
        private const int HashSize = 32;
        private readonly ITokenService tokenService;
        private readonly IUserRepository userRepository;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly int daysRefreshTokenExpiry;

        public AuthService(
            ITokenService tokenService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            int daysRefreshTokenExpiry)
        {
            this.tokenService = tokenService;
            this.userRepository = userRepository;
            this.refreshTokenRepository = refreshTokenRepository;
            this.daysRefreshTokenExpiry = daysRefreshTokenExpiry;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (await userRepository.UserExistsAsync(request.Login, cancellationToken))
                throw new ArgumentException("Пользователь с таким логином существует");

            var user = new User
            {
                LastName = request.LastName,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                Login = request.Login,
                Salt = Guid.NewGuid().ToString(),
            };

            user.Password = GetHashPassword(request.Password, user.Salt);

            await userRepository.CreateAsync(user, cancellationToken);

            return await GenerateTokens(user, cancellationToken);
        }

        private string GetHashPassword(string password, string salt)
        {
            return Convert.ToBase64String(Pbdkf2(password, salt));
        }

        private byte[] Pbdkf2(string password, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                      password: password,
                      salt: saltBytes,
                      iterations: 1,
                      hashAlgorithm: HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);

            if (user == null || user.Password != GetHashPassword(request.Password, user.Salt))
                throw new UnauthorizedAccessException("Аутентификация не пройдена");

            await refreshTokenRepository.RemoveOldTokensAsync(user.Id, cancellationToken);

            return await GenerateTokens(user, cancellationToken);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            if (!tokenService.IsTokenExpired(request.AccessToken))
                throw new SecurityTokenException("Токен доступа еще действует");

            var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

            int userId;

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                    principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out userId))
                throw new SecurityTokenException("Проблема с токеном. Не содержит claim");

            var storedRefreshToken = await refreshTokenRepository.GetValidTokenAsync(userId, request.RefreshToken, cancellationToken);
            if (storedRefreshToken == null)
                throw new SecurityTokenException("Проблема с refresh токеном");

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            var newRefreshToken = tokenService.GenerateRefreshToken();
            await refreshTokenRepository.RevokeTokenAsync(
                storedRefreshToken.Token,
                newRefreshToken);

            var newAccessToken = tokenService.GenerateAccessToken(user);

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(daysRefreshTokenExpiry)
            };

            await refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
            };
        }

        private async Task<AuthResponse> GenerateTokens(User user, CancellationToken cancellationToken)
        {
            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(daysRefreshTokenExpiry)
            };

            await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
            };
        }
    }
}
