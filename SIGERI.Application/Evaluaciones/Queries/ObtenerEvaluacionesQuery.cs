using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Evaluaciones.Queries;

public sealed record ObtenerEvaluacionesQuery : IRequest<IReadOnlyCollection<EvaluacionDto>>;
