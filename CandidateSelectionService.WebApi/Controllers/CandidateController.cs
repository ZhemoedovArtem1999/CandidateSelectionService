using CandidateSelectionService.Core.Models.Candidate;
using CandidateSelectionService.Core.Models.Filter;
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
    [SwaggerTag("API методы для работы с кандидатом")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            this.candidateService = candidateService;
        }

        /// <summary>
        /// Метод создания кандидата
        /// </summary>
        /// <param name="candidate">Информация о кандидате</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// <para>200 OK - Кандидат успешно создан</para>
        /// <para>400 BadRequest - Ошибка в переданных данных</para>
        /// <para>500 InternalServerError - Ошибка сервера</para>
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<string>, BadRequest<ValidationProblemDetails>, ProblemHttpResult>> CreateAsync([FromBody] CandidateEditDto candidate, CancellationToken cancellationToken)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                    throw new UnauthorizedAccessException("Ошибка проверки токена");

                await candidateService.CreateCandidateAsync(candidate, userId, cancellationToken);

                return TypedResults.Ok("Кандидат создан");
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

        /// <summary>
        /// Метод обновления кандидата
        /// </summary>
        /// <param name="id">Id обновляемого кандидата</param>
        /// <param name="candidate">Обновленная информация о кандидате</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// <para>200 OK - Кандидат успешно обновлен</para>
        /// <para>400 BadRequest - Ошибка в переданных данных</para>
        /// <para>500 InternalServerError - Ошибка сервера</para>
        /// </returns>
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<string>, BadRequest<ValidationProblemDetails>, ProblemHttpResult>> UpdateAsync(int id, [FromBody] CandidateEditDto candidate, CancellationToken cancellationToken)
        {
            try
            {
                await candidateService.UpdateCandidateAsync(id, candidate, cancellationToken);

                return TypedResults.Ok("Кандидат обновлен");
            }
            catch (InvalidOperationException ex)
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

        /// <summary>
        /// Получение кандидатов по заданному фильтру
        /// </summary>
        /// <param name="filter">Фильтр для выбора кандидатов</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// <para>200 OK - Успешное выполнение</para>
        /// <para>500 InternalServerError - Ошибка сервера</para>
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CandidateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<IEnumerable<CandidateDto>>, ProblemHttpResult>> GetFilterAsync([FromQuery] CandidateFilter filter, CancellationToken cancellationToken)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                    throw new UnauthorizedAccessException("Ошибка проверки токена");

                filter.SetCreateUserId(userId);

                var result = await candidateService.GetCandidatesFilterAsync(filter, cancellationToken);

                return TypedResults.Ok(result);
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
        /// <summary>
        /// Метод переноса кандидата в сотрудники
        /// </summary>
        /// <param name="candidateId">Id переносимого кандидата</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// <para>200 OK - Кандидат успешно обновлен</para>
        /// <para>400 BadRequest - Ошибка в переданных данных</para>
        /// <para>500 InternalServerError - Ошибка сервера</para>
        /// </returns>
        [HttpPost("transfer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<Ok<string>, BadRequest<ValidationProblemDetails>, ProblemHttpResult>> TransferCandidateToEmployee([FromBody] int candidateId, CancellationToken cancellationToken)
        {
            try
            {
                await candidateService.TransferCandidateToEmployee(candidateId, cancellationToken);

                return TypedResults.Ok("Кандидат успешно переведен в сотрудники");
            }
            catch (InvalidOperationException ex)
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
    }
}
