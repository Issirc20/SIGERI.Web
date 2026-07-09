using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Dependencias.Commands;

public sealed record RegistrarDependenciaActivoCommand(
    Guid ActivoOrigenId,
    Guid ActivoDestinoId,
    TipoDependenciaActivo TipoDependencia,
    string Descripcion,
    int CriticidadOperativa,
    string CreadoPor) : IRequest<Guid>;
