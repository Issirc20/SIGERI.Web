using MediatR;
using SIGERI.Domain.Enums;

namespace SIGERI.Application.Usuarios.Commands;

public sealed record RegistrarUsuarioCommand(
    string Nombre,
    string Apellido,
    string Correo,
    string Password,
    RolUsuario Rol,
    string CreadoPor) : IRequest<Guid>;
