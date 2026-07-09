using MediatR;

namespace SIGERI.Application.Usuarios.Commands;

public sealed record CambiarPasswordCommand(
    Guid UsuarioId,
    string NuevoPassword,
    string ActualizadoPor) : IRequest;
