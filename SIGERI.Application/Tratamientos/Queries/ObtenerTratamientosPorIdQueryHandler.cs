using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.DTOs;
using SIGERI.Application.Interfaces;

namespace SIGERI.Application.Tratamientos.Queries;

internal sealed class ObtenerTratamientoPorIdQueryHandler
    : IRequestHandler<ObtenerTratamientoPorIdQuery, PlanTratamientoDto?>
{
    private readonly ISigeriDbContext _context;

    public ObtenerTratamientoPorIdQueryHandler(ISigeriDbContext context)
    {
        _context = context;
    }

    public async Task<PlanTratamientoDto?> Handle(
        ObtenerTratamientoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.PlanesTratamiento
            .AsNoTracking()
            .Include(p => p.Riesgo)
            .Where(p => p.Id == request.Id)
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
            .SingleOrDefaultAsync(cancellationToken);
    }
}
