using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Dependencias.Commands;

public sealed record ActualizarDependenciaActivoCommand(
    Guid Id,
    Guid ActivoOrigenId,
    Guid ActivoDestinoId,
    TipoDependenciaActivo TipoDependencia,
    string Descripcion,
    int CriticidadOperativa,
    string ActualizadoPor) : IRequest<Guid>;
