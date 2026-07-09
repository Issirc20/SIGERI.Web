using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.ControlesMadurez.Queries;

public sealed class ObtenerControlesMadurezQueryHandler : IRequestHandler<ObtenerControlesMadurezQuery, IReadOnlyCollection<ControlMadurezDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerControlesMadurezQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ControlMadurezDto>> Handle(ObtenerControlesMadurezQuery request, CancellationToken cancellationToken)
    {
        return await _context.ControlesMadurez
            .AsNoTracking()
            .Select(x => new ControlMadurezDto(
                x.Id,
                x.Codigo,
                x.Nombre,
                x.Funcion,
                x.NivelActual,
                x.NivelObjetivo,
                x.Descripcion))
            .ToListAsync(cancellationToken);
    }
}
