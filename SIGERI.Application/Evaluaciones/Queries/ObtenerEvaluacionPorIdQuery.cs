using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Evaluaciones.Queries;

public sealed record ObtenerEvaluacionPorIdQuery(Guid Id) : IRequest<EvaluacionDto?>;
