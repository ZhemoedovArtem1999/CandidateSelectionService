using CandidateSelectionService.Core.Models.Auth;
using CandidateSelectionService.Core.Service.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateSelectionService.Auth.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            await _next(context);

            if (context.Response.StatusCode == 401 &&
                 context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    var accessToken = authHeader.Substring("Bearer ".Length).Trim();
                    if (context.Request.Headers.TryGetValue("X-Refresh-Token", out var refreshToken))
                    {
                        try
                        {
                            var newTokens = await authService.RefreshTokenAsync(
                                new RefreshTokenRequest
                                {
                                    AccessToken = accessToken,
                                    RefreshToken = refreshToken
                                }, CancellationToken.None);

                            context.Response.Headers["New-Access-Token"] = newTokens.AccessToken;
                            context.Response.Headers["New-Refresh-Token"] = newTokens.RefreshToken;

                            context.Response.StatusCode = 200;
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}