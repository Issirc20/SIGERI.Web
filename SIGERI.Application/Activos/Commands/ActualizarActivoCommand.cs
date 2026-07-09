using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Activos.Commands;

public sealed record ActualizarActivoCommand(
    Guid Id,
    string Codigo,
    string Nombre,
    string Descripcion,
    TipoActivo TipoActivo,
    string Propietario,
    string Ubicacion,
    Criticidad Criticidad,
    Guid OrganizacionId,
    string ActualizadoPor) : IRequest<Guid>;
