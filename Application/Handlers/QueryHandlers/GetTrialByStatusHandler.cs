using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialByStatusHandler : IRequestHandler<GetTrialByStatusQuery, List<ClinicalTrial>>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        public GetTrialByStatusHandler(IQueryRepository<ClinicalTrial> queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<ClinicalTrial>> Handle(GetTrialByStatusQuery request, CancellationToken cancellationToken)
        {
            return (List<ClinicalTrial>) await _queryRepository.GetTrialsAsync(request.status);
        }
    }
}
