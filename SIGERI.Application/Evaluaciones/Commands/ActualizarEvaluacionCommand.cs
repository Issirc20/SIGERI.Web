using MediatR;

namespace SIGERI.Application.Evaluaciones.Commands;

public sealed record ActualizarEvaluacionCommand(
    Guid Id,
    DateTime Fecha,
    string Observaciones,
    Guid UsuarioId,
    Guid ActivoId,
    string ActualizadoPor) : IRequest;
