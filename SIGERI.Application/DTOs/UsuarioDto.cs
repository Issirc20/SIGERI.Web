using SIGERI.Domain.Enums;

namespace SIGERI.Application.DTOs;

public sealed record UsuarioDto(
    Guid Id,
    string Nombre,
    string Apellido,
    string Correo,
    RolUsuario Rol,
    bool Estado);
