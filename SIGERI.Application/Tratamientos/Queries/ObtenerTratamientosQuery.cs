using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Tratamientos.Queries;

public sealed record ObtenerTratamientosQuery : IRequest<IReadOnlyCollection<PlanTratamientoDto>>;
