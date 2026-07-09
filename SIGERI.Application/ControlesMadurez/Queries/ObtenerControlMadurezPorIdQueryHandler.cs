using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.ControlesMadurez.Queries;

public sealed class ObtenerControlMadurezPorIdQueryHandler : IRequestHandler<ObtenerControlMadurezPorIdQuery, ControlMadurezDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerControlMadurezPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<ControlMadurezDto?> Handle(ObtenerControlMadurezPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.ControlesMadurez
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ControlMadurezDto(
                x.Id,
                x.Codigo,
                x.Nombre,
                x.Funcion,
                x.NivelActual,
                x.NivelObjetivo,
                x.Descripcion))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
