using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Riesgos.Queries;

public sealed record ObtenerRiesgoPorIdQuery(Guid Id) : IRequest<RiesgoDto?>;
