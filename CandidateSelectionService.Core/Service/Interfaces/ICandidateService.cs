using CandidateSelectionService.Core.Models.Candidate;
using CandidateSelectionService.Core.Models.Filter;

namespace CandidateSelectionService.Core.Service.Interfaces
{
    public interface ICandidateService
    {
        /// <summary>
        /// Создание кандидата
        /// </summary>
        /// <param name="candidate">Информация о создаваемом кандидате</param>
        /// <param name="userId">Id пользователя создавшего кандидата</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Ошибка в переданных данных</exception>
        Task CreateCandidateAsync(CandidateEditDto candidate, int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Обновление кандидата
        /// </summary>
        /// <param name="id">Id обновляемого кандидата</param>
        /// <param name="candidate">Данные кандидата</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Ошибка передачи неверных данных</exception>
        Task UpdateCandidateAsync(int id, CandidateEditDto candidate, CancellationToken cancellationToken);

        /// <summary>
        /// Получение кандидатов по фильтру
        /// </summary>
        /// <param name="filter">Фильтер для кандидатов</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Список отфильтрованных кандидатов</returns>
        Task<IEnumerable<CandidateDto>> GetCandidatesFilterAsync(CandidateFilter filter, CancellationToken cancellationToken);

        /// <summary>
        /// Перевод кандидата в сотрудники
        /// </summary>
        /// <param name="candidateId">Id переводимого кандидата</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Неверно переданных Id кандидата</exception>
        Task TransferCandidateToEmployee(int candidateId, CancellationToken cancellationToken);
    }
}
