using MediatR;
using SIGERI.Application.DTOs;

namespace SIGERI.Application.ControlesMadurez.Queries;

public sealed record ObtenerControlesMadurezQuery : IRequest<IReadOnlyCollection<ControlMadurezDto>>;
