using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Candidate;
using CandidateSelectionService.Core.Models.Filter;
using CandidateSelectionService.Core.Repository;
using CandidateSelectionService.Core.Service.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace CandidateSelectionService.Core.Service.Impl
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository candidateRepository;
        private readonly IDataCandidateRepository dataCandidateRepository;
        private readonly ISocialNetworkRepository socialNetworkRepository;
        private readonly IWorkScheduleRepository workScheduleRepository;
        private readonly IEmployeeRepository employeeRepository;

        public CandidateService(ICandidateRepository candidateRepository,
            IDataCandidateRepository dataCandidateRepository,
            ISocialNetworkRepository socialNetworkRepository,
            IWorkScheduleRepository workScheduleRepository,
            IEmployeeRepository employeeRepository)
        {
            this.candidateRepository = candidateRepository;
            this.dataCandidateRepository = dataCandidateRepository;
            this.socialNetworkRepository = socialNetworkRepository;
            this.workScheduleRepository = workScheduleRepository;
            this.employeeRepository = employeeRepository;
        }

        public async Task CreateCandidateAsync(CandidateEditDto candidate, int userId, CancellationToken cancellationToken)
        {
            if (!await CheckWorkScheduleAsync(candidate.WorkScheduleId, cancellationToken)) throw new InvalidOperationException("Неверно указан рабочий график");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int dataId = await dataCandidateRepository.AddAsync(new DataCandidate
                    {
                        LastName = candidate.LastName,
                        FirstName = candidate.FirstName,
                        MiddleName = candidate.MiddleName,
                        DateBirth = candidate.DateBirth,
                        Email = candidate.Email,
                        Phone = candidate.Phone,
                        Country = candidate.Country
                    }, cancellationToken);

                    await candidateRepository.AddAsync(new Candidate
                    {
                        DataId = dataId,
                        WorkScheduleId = candidate.WorkScheduleId,
                        CreatedUserId = userId,
                        LastUpdated = DateTime.UtcNow

                    }, cancellationToken);

                    if (candidate.SocialNetworks != null && candidate.SocialNetworks.Any())
                    {
                        await CreateSocialNetworksAsync(candidate.SocialNetworks, dataId, cancellationToken);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private async Task<bool> CheckWorkScheduleAsync(int id, CancellationToken cancellationToken)
        {
            var workSchedule = await workScheduleRepository.GetAsync(id, cancellationToken);
            return workSchedule == null ? false : true;
        }

        private async Task CreateSocialNetworksAsync(IEnumerable<SocialNetworkModel> socialNetworks, int dataId, CancellationToken cancellationToken)
        {
            try
            {
                await socialNetworkRepository.AddRangeAsync(socialNetworks.Select(x => new SocialNetwork
                {
                    DataCandidateId = dataId,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    DateAdded = DateTime.UtcNow,
                    TypeId = x.TypeId
                }), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("При добавлении соц. сетей произошла ошибка");
            }
        }
        public async Task UpdateCandidateAsync(int id, CandidateEditDto candidate, CancellationToken cancellationToken)
        {
            var currentCandidate = await candidateRepository.GetAsync(id, cancellationToken);

            if (currentCandidate == null) throw new InvalidOperationException($"Кандидат с id {id} не найден");

            if (!await CheckWorkScheduleAsync(candidate.WorkScheduleId, cancellationToken)) throw new InvalidOperationException("Неверно указан рабочий график");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                currentCandidate.WorkScheduleId = candidate.WorkScheduleId;
                currentCandidate.LastUpdated = DateTime.UtcNow;

                await candidateRepository.UpdateAsync(currentCandidate, cancellationToken);

                var dataCandidate = await dataCandidateRepository.GetAsync(currentCandidate.DataId, cancellationToken);

                if (dataCandidate == null) throw new InvalidOperationException("Невозможно обновить кандидата. Данные утеряны.");

                dataCandidate.LastName = candidate.LastName;
                dataCandidate.FirstName = candidate.FirstName;
                dataCandidate.MiddleName = candidate.MiddleName;
                dataCandidate.Email = candidate.Email;
                dataCandidate.Phone = candidate.Phone;
                dataCandidate.Country = candidate.Country;
                dataCandidate.DateBirth = candidate.DateBirth;

                await dataCandidateRepository.UpdateAsync(dataCandidate, cancellationToken);

                await UpdateSocialNetworkAsync(candidate.SocialNetworks, dataCandidate.Id, cancellationToken);

                scope.Complete();
            }
        }

        private async Task UpdateSocialNetworkAsync(IEnumerable<SocialNetworkModel> socialNetworks, int dataId, CancellationToken cancellationToken)
        {
            if (socialNetworks == null) return;

            var deleted = socialNetworks.Where(x => x.IsDeleted && x.Id != null);

            if (deleted != null && deleted.Any())
            {
                await socialNetworkRepository.DeleteRangeAsync(socialNetworks.Where(x => x.IsDeleted && x.Id != null).Select(x => x.Id.Value), cancellationToken);
            }

            var created = socialNetworks.Where(x => x.Id == null);

            if (created != null && created.Any())
            {
                await CreateSocialNetworksAsync(socialNetworks.Where(x => x.Id == null), dataId, cancellationToken);
            }

            var updated = await socialNetworkRepository.GetAsync(dataId, socialNetworks.Where(x => x.Id != null && !x.IsDeleted).Select(x => x.Id.Value), cancellationToken);

            foreach (var item in updated)
            {
                var update = socialNetworks.Where(x => x.Id == item.Id).First();
                item.LastName = update.LastName;
                item.FirstName = update.FirstName;
                item.TypeId = update.TypeId;
            }

            if (updated != null && updated.Any())
            {
                await socialNetworkRepository.UpdateRangeAsync(updated, cancellationToken);
            }
        }

        public async Task<IEnumerable<CandidateDto>> GetCandidatesFilterAsync(CandidateFilter filter, CancellationToken cancellationToken)
        {
            var candidates = await candidateRepository.GetFilterAsync(filter, cancellationToken);

            return candidates.Select(x => new CandidateDto(
                x.Id,
                x.DataCandidate.LastName,
                x.DataCandidate.FirstName,
                x.DataCandidate.MiddleName,
                x.DataCandidate.Email,
                x.DataCandidate.Phone,
                x.DataCandidate.Country,
                x.DataCandidate.DateBirth,
                x.WorkSchedule.Title,
                x.DataCandidate.SocialNetworks.Select(x => new SocialNetworkView(
                    x.Id,
                    x.LastName,
                    x.FirstName,
                    x.SocialNetworkType.Title
                    )).ToList()
                ));
        }

        public async Task TransferCandidateToEmployee(int candidateId, CancellationToken cancellationToken)
        {
            var currentCandidate = await candidateRepository.GetAsync(candidateId, cancellationToken);

            if (currentCandidate == null) throw new InvalidOperationException($"Кандидат с id {candidateId} не найден");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                var employee = new Employee
                {
                    DataId = currentCandidate.DataId,
                    EmploymentDate = DateTime.UtcNow,
                    WorkScheduleId = currentCandidate.WorkScheduleId
                };

                await employeeRepository.AddAsync(employee, cancellationToken);

                await candidateRepository.DeleteAsync(currentCandidate, cancellationToken);

                scope.Complete();
            }
        }
    }
}
