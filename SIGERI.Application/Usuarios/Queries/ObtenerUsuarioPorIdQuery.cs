using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Usuarios.Queries;

public sealed record ObtenerUsuarioPorIdQuery(Guid Id) : IRequest<UsuarioDto?>;
