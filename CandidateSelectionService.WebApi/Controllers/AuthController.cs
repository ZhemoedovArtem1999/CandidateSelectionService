using CandidateSelectionService.Core.Models.Auth;
using CandidateSelectionService.Core.Service.Interfaces.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CandidateSelectionService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<AuthResponse>, BadRequest<ValidationProblemDetails>, ProblemHttpResult>> Register([FromBody] Core.Models.Auth.RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await authService.RegisterAsync(request, cancellationToken);
                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex)
            {
                var problems = new ValidationProblemDetails
                {
                    Title = ex.Message,
                };
                return TypedResults.BadRequest(problems);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(detail: "Произошла непредвиденная ошибка", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult, ProblemHttpResult>> Login([FromBody] Core.Models.Auth.LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await authService.LoginAsync(request, cancellationToken);
                return TypedResults.Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return TypedResults.Unauthorized();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(detail: "Произошла непредвиденная ошибка", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<AuthResponse>, BadRequest<ValidationProblemDetails>, UnauthorizedHttpResult, ProblemHttpResult>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await authService.RefreshTokenAsync(request, cancellationToken);
                return TypedResults.Ok(response);
            }
            catch (SecurityTokenException ex)
            {
                return TypedResults.BadRequest(new ValidationProblemDetails { Title = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return TypedResults.Unauthorized();
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(detail: "Произошла непредвиденная ошибка", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
