using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.PerfilesCriticos.Commands;

public sealed record ActualizarPerfilActivoCriticoCommand(
    Guid Id,
    Guid ActivoId,
    NivelImpactoOctave ImpactoReputacion,
    NivelImpactoOctave ImpactoFinanciero,
    NivelImpactoOctave ImpactoProductividad,
    NivelImpactoOctave ImpactoLegal,
    NivelImpactoOctave ImpactoSeguridadSsoma,
    string ContenedoresTecnicos,
    string ContenedoresFisicos,
    string ContenedoresHumanos,
    string ActualizadoPor) : IRequest<Guid>;
