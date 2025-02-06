using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialByTrialIdHandler : IRequestHandler<GetTrialByTrialIdQuery, ClinicalTrial>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        public GetTrialByTrialIdHandler(IQueryRepository<ClinicalTrial> queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<ClinicalTrial> Handle(GetTrialByTrialIdQuery request, CancellationToken cancellationToken)
        {
            return (ClinicalTrial) await _queryRepository.GetByIdAsync(request.trialId);
        }
    }
}
