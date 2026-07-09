using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Dependencias.Queries;

public sealed record ObtenerDependenciaActivoPorIdQuery(Guid Id) : IRequest<DependenciaActivoDto?>;
