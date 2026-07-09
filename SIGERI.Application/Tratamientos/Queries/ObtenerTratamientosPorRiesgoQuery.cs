using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Tratamientos.Queries;

public sealed record ObtenerTratamientosPorRiesgoQuery(Guid RiesgoId)
    : IRequest<IReadOnlyCollection<PlanTratamientoDto>>;
