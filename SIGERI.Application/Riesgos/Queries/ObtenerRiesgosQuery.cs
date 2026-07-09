using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.Riesgos.Queries;

public sealed record ObtenerRiesgosQuery : IRequest<IReadOnlyCollection<RiesgoDto>>;
