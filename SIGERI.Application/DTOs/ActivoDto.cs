using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record ActivoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string Descripcion,
    TipoActivo Tipo,
    string Propietario,
    string Ubicacion,
    Criticidad Criticidad,
    Guid OrganizacionId,
    string NombreOrganizacion,
    bool Estado);
