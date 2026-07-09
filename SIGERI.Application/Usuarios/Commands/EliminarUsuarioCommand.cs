using MediatR;

namespace SIGERI.Application.Usuarios.Commands;

public sealed record EliminarUsuarioCommand(Guid Id, string ActualizadoPor) : IRequest;
