using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Riesgos.Queries;

public sealed record ObtenerRiesgosPorEvaluacionQuery(Guid EvaluacionId)
    : IRequest<IReadOnlyCollection<RiesgoDto>>;
