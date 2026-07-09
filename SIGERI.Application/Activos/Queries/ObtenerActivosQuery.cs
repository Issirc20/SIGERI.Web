using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Activos.Queries;

public sealed record ObtenerActivosQuery : IRequest<IReadOnlyCollection<ActivoDto>>;
