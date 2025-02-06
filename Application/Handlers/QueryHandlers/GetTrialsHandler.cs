using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Handlers.QueryHandlers
{
    public class GetTrialsHandler : IRequestHandler<GetTrialsQuery, List<ClinicalTrial>>
    {
        private readonly IQueryRepository<ClinicalTrial> _queryRepository;
        public GetTrialsHandler(IQueryRepository<ClinicalTrial> queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<ClinicalTrial>> Handle(GetTrialsQuery request, CancellationToken cancellationToken)
        {
            return (List<ClinicalTrial>)await _queryRepository.GetAllAsync();
        }
    }
}
