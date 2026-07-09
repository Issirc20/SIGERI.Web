using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Usuarios.Commands;

public sealed record ActualizarUsuarioCommand(
    Guid Id,
    string Nombre,
    string Apellido,
    string Correo,
    RolUsuario Rol,
    bool Estado,
    string ActualizadoPor) : IRequest;
