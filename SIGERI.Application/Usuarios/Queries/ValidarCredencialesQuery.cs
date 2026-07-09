using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Usuarios.Queries;

public sealed record ValidarCredencialesQuery(string Correo, string Password) : IRequest<UsuarioDto?>;
