using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Tratamientos.Queries;

internal sealed class ObtenerTratamientosQueryHandler
    : IRequestHandler<ObtenerTratamientosQuery, IReadOnlyCollection<PlanTratamientoDto>>
{
    private readonly ISigeriDbContext _context;

    public ObtenerTratamientosQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PlanTratamientoDto>> Handle(
        ObtenerTratamientosQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.PlanesTratamiento
            .AsNoTracking()
            .Include(p => p.Riesgo)
            .OrderByDescending(p => p.FechaCreacion)
            .Select(p => new PlanTratamientoDto(
                p.Id,
                p.RiesgoId,
                p.Riesgo.Nombre,
                p.Estrategia,
                p.Descripcion,
                p.FechaInicio,
                p.FechaTermino,
                p.Estado,
                p.CostoSalvaguarda,
                p.PorcentajeMitigacion,
                p.AleBase,
                p.AleBase * (1 - p.PorcentajeMitigacion / 100m),
                p.CostoSalvaguarda > 0
                    ? ((p.AleBase - p.AleBase * (1 - p.PorcentajeMitigacion / 100m) - p.CostoSalvaguarda) / p.CostoSalvaguarda) * 100m
                    : 0m,
                p.Controles.Count))
            .ToListAsync(cancellationToken);
    }
}
