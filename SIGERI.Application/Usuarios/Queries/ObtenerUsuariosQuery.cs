using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Usuarios.Queries;

public sealed record ObtenerUsuariosQuery : IRequest<IReadOnlyCollection<UsuarioDto>>;
