using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Activos.Queries;

public sealed record ObtenerActivoPorIdQuery(Guid Id) : IRequest<ActivoDto?>;
