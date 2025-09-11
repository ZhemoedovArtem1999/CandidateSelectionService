using CandidateSelectionService.Core.Models.Search;
using CandidateSelectionService.Core.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CandidateSelectionService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [SwaggerTag("API методы для работы с поиском")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;
        private readonly ISendDataService sendDataService;

        public SearchController(ISearchService searchService, ISendDataService sendDataService)
        {
            this.searchService = searchService;
            this.sendDataService = sendDataService;
        }
        /// <summary>
        /// Метод поиска кандидатов и сотрудников
        /// </summary>
        /// <param name="search">Критерии поиска</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<SendData>, BadRequest<ValidationProblemDetails>, ProblemHttpResult>> SearchAsync(SearchDataDto search, CancellationToken cancellationToken)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                    throw new UnauthorizedAccessException("Ошибка проверки токена");

                var result = await searchService.SearchEntitiesAsync(search, userId, cancellationToken);

                await sendDataService.SendMessageAsync(result, cancellationToken);

                return TypedResults.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                var problems = new ValidationProblemDetails
                {
                    Title = ex.Message,
                };
                return TypedResults.BadRequest(problems);
            }
            catch (UnauthorizedAccessException ex)
            {
                return TypedResults.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(detail: "Произошла непредвиденная ошибка", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
