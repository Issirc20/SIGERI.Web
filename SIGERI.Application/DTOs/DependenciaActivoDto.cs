using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record DependenciaActivoDto(
    Guid Id,
    Guid ActivoOrigenId,
    string ActivoOrigen,
    Guid ActivoDestinoId,
    string ActivoDestino,
    TipoDependenciaActivo TipoDependencia,
    string Descripcion,
    int CriticidadOperativa);
