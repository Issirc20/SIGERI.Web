using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.PerfilesCriticos.Queries;

public sealed record ObtenerPerfilActivoCriticoPorIdQuery(Guid Id) : IRequest<PerfilActivoCriticoDto?>;
