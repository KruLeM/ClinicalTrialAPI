﻿using Application.DTOs;
using MediatR;

namespace Application.Queries
{
    public record GetTrialsQuery : IRequest<List<ClinicalTrialDTO>>;
}
