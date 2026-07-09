using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record RiesgoDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    int ProbabilidadValor,
    string ProbabilidadDescripcion,
    int ImpactoValor,
    string ImpactoDescripcion,
    int PuntajeRiesgo,
    string NivelRiesgo,
    EstadoRiesgo Estado,
    Guid EvaluacionId,
    Guid AmenazaId,
    string AmenazaNombre,
    Guid VulnerabilidadId,
    string VulnerabilidadNombre);
