using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record PerfilActivoCriticoDto(
    Guid Id,
    Guid ActivoId,
    string Activo,
    NivelImpactoOctave ImpactoReputacion,
    NivelImpactoOctave ImpactoFinanciero,
    NivelImpactoOctave ImpactoProductividad,
    NivelImpactoOctave ImpactoLegal,
    NivelImpactoOctave ImpactoSeguridadSsoma,
    string ContenedoresTecnicos,
    string ContenedoresFisicos,
    string ContenedoresHumanos);
