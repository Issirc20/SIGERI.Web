using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.ControlesMadurez.Queries;

public sealed record ObtenerControlMadurezPorIdQuery(Guid Id) : IRequest<ControlMadurezDto?>;
