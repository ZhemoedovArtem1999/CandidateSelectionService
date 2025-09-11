using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Filter;
using CandidateSelectionService.Core.Models.Search;
using CandidateSelectionService.Core.Repository;
using CandidateSelectionService.Core.Repository.Auth;
using CandidateSelectionService.Core.Service.Interfaces;
using System.Transactions;

namespace CandidateSelectionService.Core.Service.Impl
{
    public class SearchService : ISearchService
    {
        private const string CANDIDATE_TYPE = "Список кандидатов";
        private const string EMPLOYEE_TYPE = "Сотрудники";

        private readonly ICandidateRepository candidateRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IUserRepository userRepository;
        private readonly IVerificationRepository verificationRepository;
        private readonly IVerificationEventRepository verificationEventRepository;
        private readonly IVerificationEventResultRepository verificationEventResultRepository;

        public SearchService(
            ICandidateRepository candidateRepository,
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IVerificationRepository verificationRepository,
            IVerificationEventRepository verificationEventRepository,
            IVerificationEventResultRepository verificationEventResultRepository)
        {
            this.candidateRepository = candidateRepository;
            this.employeeRepository = employeeRepository;
            this.userRepository = userRepository;
            this.verificationRepository = verificationRepository;
            this.verificationEventRepository = verificationEventRepository;
            this.verificationEventResultRepository = verificationEventResultRepository;
        }

        public async Task<SendData> SearchEntitiesAsync(SearchDataDto search, int userId, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null) throw new InvalidOperationException("Пользователь не найден");

            var candidateTask = candidateRepository.GetFilterAsync(new CandidateFilter { LastName = search.LastName, FirstName = search.FirstName, PageNumber = 0 }, cancellationToken);
            var employeeTask = employeeRepository.GetFilterAsync(new EmployeeFilter { LastName = search.LastName, FirstName = search.FirstName }, cancellationToken);

            await Task.WhenAll(candidateTask, employeeTask);

            var candidateResults = candidateTask.Result;
            var employeeResults = employeeTask.Result;

            int verificationId;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                var verification = new Verification
                {
                    UserName = user.LastName + " " + user.FirstName + " " + user.MiddleName,
                    Date = DateTime.UtcNow,
                    SearchData = search.LastName + " " + search.FirstName
                };

                verificationId = await verificationRepository.AddAsync(verification, cancellationToken);

                if (candidateResults.Any())
                {
                    var verificationEvent = new VerificationEvent
                    {
                        VerificationId = verificationId,
                        Type = CANDIDATE_TYPE
                    };

                    var verificationEventId = await verificationEventRepository.AddAsync(verificationEvent, cancellationToken);

                    var results = candidateResults.Select(x => new VerificationEventResult
                    {
                        VerificationEventId = verificationEventId,
                        EntityId = x.Id,
                        LastName = x.DataCandidate.LastName,
                        FirstName = x.DataCandidate.FirstName,
                        MiddleName = x.DataCandidate.MiddleName,
                        DateBirth = x.DataCandidate.DateBirth,
                        Email = x.DataCandidate.Email,
                        Phone = x.DataCandidate.Phone,
                        Country = x.DataCandidate.Country,
                        WorkSchedule = x.WorkSchedule.Title
                    });

                    await verificationEventResultRepository.AddRangeAsync(results, cancellationToken);
                }

                if (employeeResults.Any())
                {
                    var verificationEvent = new VerificationEvent
                    {
                        VerificationId = verificationId,
                        Type = EMPLOYEE_TYPE
                    };

                    var verificationEventId = await verificationEventRepository.AddAsync(verificationEvent, cancellationToken);

                    var results = employeeResults.Select(x => new VerificationEventResult
                    {
                        VerificationEventId = verificationEventId,
                        EntityId = x.Id,
                        LastName = x.DataCandidate.LastName,
                        FirstName = x.DataCandidate.FirstName,
                        MiddleName = x.DataCandidate.MiddleName,
                        DateBirth = x.DataCandidate.DateBirth,
                        Email = x.DataCandidate.Email,
                        Phone = x.DataCandidate.Phone,
                        Country = x.DataCandidate.Country,
                        WorkSchedule = x.WorkSchedule.Title
                    });

                    await verificationEventResultRepository.AddRangeAsync(results, cancellationToken);
                }

                scope.Complete();
            }

            var sendData = await FillDataSendAsync(verificationId, cancellationToken);

            return sendData;
        }

        private async Task<SendData> FillDataSendAsync(int verificationId, CancellationToken cancellationToken)
        {
            var verification = await verificationRepository.GetByIdAsync(verificationId, cancellationToken);

            var sendData = new SendData();
            sendData.UserName = verification.UserName;
            sendData.Date = verification.Date.ToString("dd.MM.yyyy HH:mm:ss");
            sendData.SearchData = verification.SearchData;

            var candidates = verification.VerificationEvents
                .Where(x => x.Type == CANDIDATE_TYPE).FirstOrDefault()?.VerificationEventResults;

            if (candidates != null && candidates.Any())
            {
                sendData.Candidates = candidates.Select(x => new EntityItem
                {
                    EntityId = x.EntityId,
                    Name = x.LastName + " " + x.FirstName + " " + x.MiddleName,
                    DateBirth = x.DateBirth.ToString("dd.MM.yyyy"),
                    Email = x.Email,
                    Phone = x.Phone,
                    Country = x.Country,
                    WorkSchedule = x.WorkSchedule
                });
            }

            var employees = verification.VerificationEvents
                .Where(x => x.Type == EMPLOYEE_TYPE).FirstOrDefault()?.VerificationEventResults;

            if (employees != null && employees.Any())
            {
                sendData.Employess = employees.Select(x => new EntityItem
                {
                    EntityId = x.EntityId,
                    Name = x.LastName + " " + x.FirstName + " " + x.MiddleName,
                    DateBirth = x.DateBirth.ToString("dd.MM.yyyy"),
                    Email = x.Email,
                    Phone = x.Phone,
                    Country = x.Country,
                    WorkSchedule = x.WorkSchedule
                });
            }

            return sendData;
        }
    }
}
