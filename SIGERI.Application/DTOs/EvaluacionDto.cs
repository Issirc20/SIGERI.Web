namespace SIGERI.Application.DTOs;

public sealed record EvaluacionDto(
    Guid Id,
    DateTime Fecha,
    string Observaciones,
    Guid UsuarioId,
    string UsuarioNombre,
    Guid ActivoId,
    string ActivoNombre,
    int TotalRiesgos);
