using MediatR;

namespace SIGERI.Application.Evaluaciones.Commands;

public sealed record RegistrarEvaluacionCommand(
    DateTime Fecha,
    string Observaciones,
    Guid UsuarioId,
    Guid ActivoId,
    string CreadoPor) : IRequest<Guid>;
