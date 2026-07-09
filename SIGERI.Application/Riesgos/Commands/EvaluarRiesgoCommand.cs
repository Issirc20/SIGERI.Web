using MediatR;

namespace SIGERI.Application.Riesgos.Commands;

public sealed record EvaluarRiesgoCommand(
    Guid ActivoId,
    Guid EvaluacionId,
    Guid AmenazaId,
    Guid VulnerabilidadId,
    int Probabilidad,
    int Impacto,
    string NombreRiesgo,
    string DescripcionRiesgo,
    string CreadoPor) : IRequest<Guid>;
